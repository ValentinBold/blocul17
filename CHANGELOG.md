# Jurnal de modificări

## 0.7.0 — 16 octombrie 2026

- Panou P pentru planificarea evadării: file Gard, Tunel și Program, cu timpul oprit.
- Inventarul planului separă materialele și uneltele din buzunare, birou, ascunzătoare și depozitul de confiscări.
- Următorul pas se adaptează la intelect, energie, unelte, tunel, gard, rutină, urmărire și percheziția apropiată.
- Program zilnic cu orele atelierului, ghișeului, apelurilor, percheziției, stingerii și următorul eveniment.
- Victor oferă sfaturi contextuale și acces direct la plan.
- Salvarea rămâne v5; planul este calculat din progres, fără migrare nouă.
- 255 aserțiuni trecute și cinci trasee complete păstrate; probă Windows Forms pentru filele planului, pauză și anularea așteptării accelerate.

## 0.6.0 — 15 octombrie 2026

- Sistem de contrabandă: pilă, sârmă, clește și lopată, marcate cu ! în inventar și recipiente.
- Detector la atelier: +35 suspiciune pe trecere; pilă și sârmă au permis numai 09:00–12:00.
- Percheziție zilnică a biroului la 18:30, anunțată la 18:00; ascunzătoarea și buzunarele sunt protejate de această verificare. Așteptarea accelerată se oprește la eveniment.
- Depozit persistent pentru contrabanda confiscată la percheziție sau capturare. Ghișeu lângă cantină, 12:00–22:00, cu recuperare contra cost.
- Panou K pentru securitate, marcaje pe hartă și meniu de recuperare cu timpul oprit.
- Salvare v5 separată, cu import v1–v4 și păstrarea originalelor, inclusiv ascunzătoarea din 0.4/0.5.
- 217 aserțiuni și cinci scenarii complete de evadare; scenariul nou acoperă muncă, ascundere, percheziție și recuperare. Probă automată Windows Forms pentru noile meniuri.

## 0.5.0 — 15 octombrie 2026

- T sau butonul Așteaptă accelerează simularea de 20 de ori până la următoarea schimbare de program.
- Patrulele, deținuții, energia și ceasul folosesc aceiași pași de simulare; timpul nu este sărit instantaneu.
- Oprire automată la program nou, miezul nopții sau stare nesigură. Mișcarea, interacțiunile, meniurile și pierderea focusului anulează accelerarea.
- Așteptarea cere prezență la program, suspiciune sub 25, lipsa urmăririi și a unei acțiuni în curs.
- Formatul de salvare rămâne v4; accelerarea nu se reia după încărcare.
- 164 de aserțiuni, patru scenarii complete de evadare și verificarea comenzii de așteptare în fereastra Windows.

## 0.4.1 — 14 octombrie 2026

- Prima lansare publică pe GitHub, cu licență MIT, documentație și verificări automate.
- Corectată generarea arhivei pe Windows PowerShell 5.1: intrările ZIP folosesc separatori portabili și sunt verificate prin SHA-256.
- Păstrează mecanicile și formatul de salvare v4 din 0.4.0. Nu cere migrarea unei salvări 0.4.0.
- 143 de aserțiuni și proba Windows Forms trec înainte de publicare.

## 0.4.0 — 14 octombrie 2026

- E deschide recipiente cu alegerea obiectului, șase rânduri pe pagină și timp oprit.
- Birou cu depunere selectivă și ascunzătoare separată pentru trei obiecte, păstrate la capturare.
- Depozitarea rapidă Q / click dreapta cere apropierea de biroul propriu.
- Accesul și transferurile sunt blocate prin ziduri, de la distanță, în acțiune, în urmărire sau la suspiciune mare.
- Salvare v4 în fișier separat, cu import al formatelor v1–v3 și păstrarea originalelor.
- 143 aserțiuni, trei trasee complete și verificarea transferurilor/pauzei în fereastra Windows.

## 0.3.0 — 14 octombrie 2026

- A doua rută de evadare: trapă în bibliotecă, lopată, trei etape de săpat și ieșire nocturnă.
- Rețetă de lopată la 40 intelect și meniu cu două unelte, tastele 1 / Enter și 2.
- Progres persistent al tunelului; mișcarea anulează etapa curentă, capturarea astupă tunelul.
- Săpatul observat crește suspiciunea; ieșirea este blocată în urmărire sau la minimum 60 suspiciune.
- Salvare v3 separată, cu import v2/v1 fără modificarea originalelor.
- 112 aserțiuni, trei trasee complete și testul comenzii pentru lopată în bucla Windows Forms.

## 0.2.0 — 14 octombrie 2026

- Credite, muncă la atelier între 09:00 și 12:00: 12 credite/piesă, 15 energie, 6 secunde, maximum 3 piese pe zi.
- Cinci deținuți identificați: Matei, Radu, Ion, Dima și Victor. F deschide dialogul cu cel apropiat; distanța, zidurile și urmărirea sunt verificate.
- Trei favoruri zilnice, unul activ simultan, predare consumabilă și plată unică. Renunțare din jurnal.
- Radu vinde și cumpără materiale, cu prețuri și stoc persistente. Tranzacțiile refuzate nu consumă obiecte sau bani. Stocul revine la două bucăți din fiecare tip la miezul nopții.
- Portofel, progres de muncă, indicatoare NPC, magazin și jurnal extins în interfață. Dialogurile opresc timpul.
- Format de salvare v2 și import al unei salvări v1, păstrând fișierul original. Salvările noi folosesc save-v2.xml.
- Limită explicită de 100 obiecte în birou, pentru a preveni producerea unei salvări care depășește validarea.
- 80 de aserțiuni, două trasee complete de evadare și probă Windows Forms cu pauză în dialog și executarea comenzilor de favor.

## 0.1.0 — 14 octombrie 2026

Prima versiune Windows jucabilă: o hartă originală, două patrule, cinci deținuți, program accelerat, inventar cu șase locuri, recipiente, studiu, hrană, odihnă, crafting de clește, tăiere de gard și evadare.

Adăugate suspiciune, vedere blocată de ziduri, urmărirea ultimei poziții văzute, capturare, refacerea materialelor și ecran final. Include salvare XML cu validare, pauză la pierderea focusului, ecran complet, ghid și jurnal.

În verificare au fost corectate accesul blocat de compartimentarea celulelor, temporizarea urmăririi dependentă de numărul de cadre și pierderea poziției patrulelor la încărcare. Livrarea include codul sursă și teste reproductibile.
