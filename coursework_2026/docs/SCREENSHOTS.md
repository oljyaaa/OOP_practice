# Скріншоти роботи програми

Знято з реального запуску `PublishingHouse.UI` (.NET 10). Ввід подавався по рядку скриптом [tools/shots.js](tools/shots.js)
за сценаріями з [tools/scenarios.json](tools/scenarios.json), вивід відрендерено у вигляді вікна терміналу.
Введені користувачем значення виділені жирним і підкресленням; кольори повідомлень такі самі, як у `ConsoleHelper`.
Повний текст кожного запуску — у файлах `.txt` поруч зі скріншотами.

| № | Файл | Що демонструє |
|---|---|---|
| 1 | [01_authors_add_validation.png](screenshots/01_authors_add_validation.png) | Додавання авторів; валідація імені, email, роялті |
| 2 | [02_editors_add_a.png](screenshots/02_editors_add_a.png) | Редактор: зайнятий email, нечислова і нульова ставка |
| 3 | [02_editors_add_b.png](screenshots/02_editors_add_b.png) | Другий редактор, список редакторів |
| 4 | [03_manuscripts_submit_a.png](screenshots/03_manuscripts_submit_a.png) | Подання рукопису: порожня назва, кількість сторінок поза межами |
| 5 | [03_manuscripts_submit_b.png](screenshots/03_manuscripts_submit_b.png) | Подання ще двох рукописів |
| 6 | [04_stages_workflow_a.png](screenshots/04_stages_workflow_a.png) | Призначення редактора, перехід на рецензію, InvalidStageTransitionException |
| 7 | [04_stages_workflow_b.png](screenshots/04_stages_workflow_b.png) | Схвалення рецензії, етапи Коректура → Верстка → Готовий до друку |
| 8 | [04_stages_workflow_c.png](screenshots/04_stages_workflow_c.png) | Детальна інформація про рукопис |
| 9 | [05_review_rejected_a.png](screenshots/05_review_rejected_a.png) | Спроба схвалити рукопис без редактора |
| 10 | [05_review_rejected_b.png](screenshots/05_review_rejected_b.png) | ManuscriptRejectedException, відхилений рукопис не має наступного етапу |
| 11 | [06_print_runs_a.png](screenshots/06_print_runs_a.png) | InvalidPrintRunSizeException (тираж 50 прим.) |
| 12 | [06_print_runs_b.png](screenshots/06_print_runs_b.png) | Тираж 2000 прим. і калькуляція собівартості та прибутку |
| 13 | [06_print_runs_c.png](screenshots/06_print_runs_c.png) | Збитковий повторний тираж, список тиражів |
| 14 | [07_reports.png](screenshots/07_reports.png) | Некоректний вибір пункту меню; звіти: рейтинг за прибутком, загальний прибуток, етапи |
| 15 | [08_sort_filter_search_a.png](screenshots/08_sort_filter_search_a.png) | Сортування рукописів за сторінками (IComparer) та етапом |
| 16 | [08_sort_filter_search_b.png](screenshots/08_sort_filter_search_b.png) | Фільтр за етапом |
| 17 | [08_sort_filter_search_c.png](screenshots/08_sort_filter_search_c.png) | Пошук за частиною назви |
| 18 | [09_delete_protection_a.png](screenshots/09_delete_protection_a.png) | Заборона видалити автора з рукописами |
| 19 | [09_delete_protection_b.png](screenshots/09_delete_protection_b.png) | Заборона видалити редактора, закріпленого за рукописом |
| 20 | [09_delete_protection_c.png](screenshots/09_delete_protection_c.png) | Видалення рукопису, ручне збереження |
| 21 | [10_restart_load.png](screenshots/10_restart_load.png) | Повторний запуск: дані завантажено з JSON |
| 22 | [11_corrupted_file.png](screenshots/11_corrupted_file.png) | Пошкоджений JSON: резервна копія .bak і порожня база |

## Як перегенерувати

```bash
# потрібні Node.js і Google Chrome; puppeteer-core ставиться у тимчасову папку
npm i puppeteer-core@22.15.0
curl -sfLo mermaid.min.js https://cdn.jsdelivr.net/npm/mermaid@10.9.1/dist/mermaid.min.js
node docs/tools/uml.js docs/uml docs/uml
dotnet build PublishingHouse.UI -o /tmp/ph
node docs/tools/shots.js /tmp/ph/PublishingHouse.UI.dll <порожня_папка> docs/screenshots docs/tools/scenarios.json
```
