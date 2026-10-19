# Blocul 17

[![Windows checks](https://github.com/ValentinBold/blocul17/actions/workflows/ci.yml/badge.svg)](https://github.com/ValentinBold/blocul17/actions/workflows/ci.yml)
[![Latest release](https://img.shields.io/github/v/release/ValentinBold/blocul17)](https://github.com/ValentinBold/blocul17/releases/latest)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

Joc original de evadare pentru **Windows**, în română. Studiază rutina închisorii, lucrează, fă favoruri și pregătește o ieșire prin gard sau prin tunel.

**[Descarcă jocul din Releases](https://github.com/ValentinBold/blocul17/releases/latest)** · [Raportează un bug](https://github.com/ValentinBold/blocul17/issues/new/choose) · [Istoric versiuni](CHANGELOG.md)

![Jocul Blocul 17](docs/images/game.png)

## Cum joci

1. Din ultima versiune, descarcă arhiva `Blocul17_Windows_v….zip` din **Assets**. Arhivele automate „Source code” conțin sursele, nu jocul compilat.
2. Extrage arhiva și deschide `Blocul17.exe`.
3. Folosește **WASD**, **E** pentru interacțiuni, **F** pentru conversații și **C** pentru unelte. **F1** deschide ghidul.

Nu necesită instalare, browser sau internet pentru joc. Folosește .NET Framework 4.x și Windows Forms. Executabilul este nesemnat și nu cere drepturi de administrator. Versiunea curentă este un prototip; vezi [verificările și limitele](VERIFICARE.md).

## Ce include 0.7.0

- Închisoare originală, patrule cu vedere și urmărire, program zilnic.
- Inventar, recipiente cu alegerea obiectelor și ascunzătoare protejată de trei locuri.
- Studiu, crafting, muncă plătită, favoruri și comerț.
- Evadare prin gard sau printr-un tunel fix cu ieșire nocturnă.
- Așteptare accelerată ×20 cu **T**, oprită automat la schimbarea programului.
- Detector la atelier, percheziție zilnică și recuperarea contrabandei confiscate.
- Panou **K · Securitate**, cu regulile și starea controlului.
- **P · Plan**: pași contextuali pentru gard/tunel, inventarul materialelor pe locuri și programul zilei. Victor oferă sfaturi adaptate progresului.
- Salvare locală și import al salvărilor 0.1–0.5.

![Securitate și contrabandă](docs/images/security.png)

Percheziția biroului are loc la 18:30. Ascunzătoarea păstrează trei obiecte în siguranță; contrabanda găsită se recuperează la ghișeul de lângă cantină. Vezi [regulile complete](CITESTE_MA.txt) și [planul update-ului](UPDATE_0_6_PLAN.md).

![Planul de evadare](docs/images/planner.png)

Apasă **P**, apoi **1** pentru gard, **2** pentru tunel sau **3** pentru program. Consultarea planului oprește timpul și nu mută obiecte.

## Actualizări și salvări

Versiunile noi apar în **Releases**, cu note despre schimbări și un ZIP Windows. Jocul nu are actualizator automat: descarci și extragi noua versiune într-un dosar separat. Poți urmări repository-ul prin **Watch → Custom → Releases**.

Salvarea 0.6/0.7 se află în `%LOCALAPPDATA%\Blocul17\save-v5.xml`. Dacă lipsește, jocul încearcă importul din `save-v4.xml` (0.4/0.5), `save-v3.xml`, `save-v2.xml`, apoi `save.xml`, fără modificarea originalului. Nu există import invers. Închide jocul înainte de a schimba versiunea și păstrează o copie a salvării dacă vrei să revii ulterior.

## Dezvoltare

Ai nevoie de Windows și compilatorul .NET Framework 4.x disponibil în `Windows\Microsoft.NET\Framework64`. Nu sunt necesare pachete NuGet.

```powershell
.\build.ps1
.\test.ps1
.\package.ps1
```

`test.ps1` compilează și rulează verificările motorului, cele cinci scenarii complete și o probă Windows Forms ascunsă. Jurnalele și imaginile se generează în `qa/`. Testarea automată nu înlocuiește verificarea manuală pe alte calculatoare.

`package.ps1` testează înainte să genereze ZIP-ul și hash-urile SHA-256. Exportul local implicit: `D:\04_DOWNLOADS\Blocul17_GitHub_Releases`. În CI se folosește un dosar temporar.

Pentru schimbări: [CONTRIBUTING.md](CONTRIBUTING.md). Pentru lansarea unei versiuni: [RELEASING.md](RELEASING.md). Pentru mecanici: [DESIGN.md](DESIGN.md), [ghidul jucătorului](CITESTE_MA.txt).

## Proveniență și licență

Proiect independent, inspirat de genul jocurilor de evadare și de The Escapists; nu este afiliat cu autorii jocului comercial. Harta, grafica procedurală și regulile acestui prototip sunt proprii. Nu include resurse comerciale, cod upstream, hărți comerciale sau binare J2ME.

Codul și grafica procedurală sunt publicate sub [licența MIT](LICENSE). Fișierele din `fixtures/` sunt salvări sintetice pentru teste, nu salvări personale.
