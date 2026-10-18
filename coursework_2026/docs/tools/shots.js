// Запускає консольну програму, подає ввід по рядку і рендерить транскрипт у PNG у стилі терміналу.
const { spawn } = require('child_process');
const puppeteer = require('puppeteer-core');
const fs = require('fs'), path = require('path');
const [dll, workDir, outDir, scenariosFile] = process.argv.slice(2);
const scenarios = JSON.parse(fs.readFileSync(scenariosFile, 'utf8'));

function run(inputs) {
  return new Promise(resolve => {
    const p = spawn('dotnet', [dll], { cwd: workDir });
    let transcript = '', timer = null, i = 0;
    const next = () => {
      if (i < inputs.length) { const line = inputs[i++]; transcript += '\u0001' + line + '\u0002\n'; p.stdin.write(line + '\n'); }
      else p.stdin.end();
    };
    p.stdout.on('data', d => { transcript += d.toString(); clearTimeout(timer); timer = setTimeout(next, 250); });
    p.stderr.on('data', d => { transcript += d.toString(); });
    p.on('close', () => { clearTimeout(timer); resolve(transcript); });
  });
}

const esc = s => s.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
function toHtml(t, title, narrow = false) {
  const lines = t.split('\n').map(line => {
    let h = esc(line).replace(/\u0001(.*?)\u0002/g, '<span class="in">$1</span>');
    if (/✔/.test(line)) h = h.replace(/(✔.*)$/, '<span class="ok">$1</span>');
    else if (/✖/.test(line)) h = h.replace(/(✖.*)$/, '<span class="err">$1</span>');
    else if (/^! |: ! /.test(line) || / ! /.test(line)) h = h.replace(/(! .*)$/, '<span class="warn">$1</span>');
    if (/^=====/.test(line)) h = `<span class="hdr">${h}</span>`;
    return h;
  });
  return `<!DOCTYPE html><html><head><meta charset="utf-8"><style>
    body{margin:0;background:#fff;padding:16px;font-family:Menlo,monospace}
    .win{display:inline-block;background:#1e1e1e;border-radius:10px;box-shadow:0 4px 18px rgba(0,0,0,.35);min-width:820px}
    .narrow .win{min-width:0} .narrow pre{white-space:pre-wrap;max-width:760px;word-break:break-word}
    .bar{height:30px;background:#3a3a3a;border-radius:10px 10px 0 0;display:flex;align-items:center;padding-left:12px;gap:8px;color:#bbb;font:13px -apple-system,Arial}
    .dot{width:12px;height:12px;border-radius:50%} .t{margin-left:14px}
    pre{margin:0;padding:14px 18px;color:#d4d4d4;font:14px/1.45 Menlo,monospace;white-space:pre}
    .in{color:#fff;font-weight:bold;text-decoration:underline;text-decoration-color:#666}
    .ok{color:#4ec94e}.err{color:#f05b5b}.warn{color:#e5c33b}.hdr{color:#3ec7d6}
  </style></head><body class="${narrow ? 'narrow' : ''}"><div class="win"><div class="bar"><span class="dot" style="background:#ff5f57"></span><span class="dot" style="background:#febc2e"></span><span class="dot" style="background:#28c840"></span><span class="t">PublishingHouse.UI — ${esc(title)}</span></div><pre>${lines.join('\n').replace(/\n+$/, '')}</pre></div></body></html>`;
}

(async () => {
  const browser = await puppeteer.launch({ executablePath: '/Applications/Google Chrome.app/Contents/MacOS/Google Chrome', headless: 'new' });
  const page = await browser.newPage();
  await page.setViewport({ width: 1400, height: 900, deviceScaleFactor: 2 });
  for (const sc of scenarios) {
    if (sc.before) for (const [f, c] of Object.entries(sc.before)) fs.writeFileSync(path.join(workDir, f), c);
    let t = await run(sc.inputs);
    if (sc.from) { const k = t.indexOf(sc.from); if (k >= 0) t = t.slice(t.lastIndexOf('\n', k) + 1); }
    if (sc.to) { const k = t.indexOf(sc.to); if (k >= 0) t = t.slice(0, t.indexOf('\n', k) + 1); }
    t = t.replace(new RegExp(workDir.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'), 'g'), '~/coursework');
    fs.writeFileSync(path.join(outDir, sc.name + '.txt'), t.replace(/[\u0001\u0002]/g, ''));
    // Розбиття довгого транскрипту: кожна частина починається з заголовка меню перед маркером
    const parts = [];
    let rest = t;
    for (const marker of sc.splitAt || []) {
      const k = rest.indexOf(marker);
      if (k < 0) { console.log('marker not found', sc.name, marker); continue; }
      const h = rest.lastIndexOf('\n=====', k);
      const cut = h >= 0 ? h + 1 : rest.lastIndexOf('\n', k) + 1;
      parts.push(rest.slice(0, cut)); rest = rest.slice(cut);
    }
    parts.push(rest);
    // Кадри для пояснювальної записки: від заголовка меню перед маркером `from` до рядка з маркером `to`
    fs.mkdirSync(path.join(outDir, 'report'), { recursive: true });
    for (const crop of sc.crops || []) {
      const k = t.indexOf(crop.from);
      if (k < 0) { console.log('crop from not found', crop.name, crop.from); continue; }
      const h = crop.noHeader ? -1 : t.lastIndexOf('\n=====', k);
      const start = h >= 0 ? h + 1 : t.lastIndexOf('\n', k) + 1;
      const e = t.indexOf(crop.to, k);
      if (e < 0) { console.log('crop to not found', crop.name, crop.to); continue; }
      const nl = t.indexOf('\n', e);
      await page.setContent(toHtml(t.slice(start, nl < 0 ? t.length : nl), sc.title, true));
      const el = await page.$('.win');
      await el.screenshot({ path: path.join(outDir, 'report', crop.name + '.png') });
      const b = await el.boundingBox();
      console.log('  crop', crop.name, Math.round(b.width) + 'x' + Math.round(b.height));
    }
    for (let n = 0; n < parts.length; n++) {
      const name = parts.length > 1 ? `${sc.name}_${'abcdef'[n]}` : sc.name;
      await page.setContent(toHtml(parts[n], sc.title + (parts.length > 1 ? ` (${n + 1}/${parts.length})` : '')));
      const el = await page.$('.win');
      await el.screenshot({ path: path.join(outDir, name + '.png') });
      const b = await el.boundingBox();
      console.log(name, Math.round(b.width) + 'x' + Math.round(b.height));
    }
  }
  await browser.close();
})();
