const puppeteer = require('puppeteer-core');
const fs = require('fs'), path = require('path');
const [srcDir, outDir] = process.argv.slice(2);
(async () => {
  const browser = await puppeteer.launch({ executablePath: '/Applications/Google Chrome.app/Contents/MacOS/Google Chrome', headless: 'new' });
  const page = await browser.newPage();
  await page.setViewport({ width: 2200, height: 1400, deviceScaleFactor: 2 });
  for (const f of fs.readdirSync(srcDir).filter(f => f.endsWith('.mmd')).sort()) {
    const code = fs.readFileSync(path.join(srcDir, f), 'utf8');
    await page.setContent(`<!DOCTYPE html><html><head><meta charset="utf-8">
      <style>body{margin:0;background:#fff;font-family:Arial} #d{display:inline-block;padding:24px}</style></head>
      <body><div id="d"><pre class="mermaid">${code.replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;')}</pre></div>
</body></html>`);
    await page.addScriptTag({ path: path.join(__dirname, 'mermaid.min.js') });
    await page.evaluate(async () => {
      mermaid.initialize({ startOnLoad: false, theme: 'default', securityLevel: 'loose',
        themeVariables: { fontFamily: 'Arial', fontSize: '15px' }, class: { useMaxWidth: false }, flowchart: { useMaxWidth: false }, state: { useMaxWidth: false } });
      await mermaid.run();
    });
    const err = await page.$('.error-icon');
    if (err) { console.log('ERROR in', f); continue; }
    const el = await page.$('#d');
    const out = path.join(outDir, f.replace('.mmd', '.png'));
    await el.screenshot({ path: out });
    const b = await el.boundingBox();
    console.log(f, Math.round(b.width) + 'x' + Math.round(b.height));
  }
  await browser.close();
})();
