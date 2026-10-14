# Blocul 17 — verificarea versiunii 0.4.0

14 octombrie 2026. Aplicație C# / Windows Forms, compilată cu compilatorul .NET Framework disponibil local. Nu folosește browser sau servicii web.

**Rezultat: 143 de aserțiuni trecute**, plus testul buclei Windows Forms. Jurnalele sunt incluse. Aserțiunile includ verificări individuale ale locurilor din inventar; numărul nu reprezintă 143 de partide independente.

## Recipiente și ascunzătoare

31 de aserțiuni noi verifică selectarea exactă a obiectelor, refuzul accesului de la distanță/prin ziduri/în urmărire, capacitățile inventarului și compartimentelor, lipsa pierderilor la transfer refuzat, păstrarea ascunzătorii la capturare, recuperarea ulterioară, resetarea la partidă nouă și persistența la salvare. Pentru scenariul de confiscare se apelează direct Caught(); nu este o partidă naturală cu capturare.

Sunt verificate pagina 17/17 a unui birou cu 100 obiecte, recuperarea ultimului obiect, date ascunse cu tip necunoscut sau prea multe obiecte și normalizarea de către serializer a unei liste XML nil la o listă goală sigură. Cele trei trasee complete anterioare au fost rulate folosind noua deschidere a recipientului și transferul explicit al obiectului.

Salvarea sintetică save-v3.xml a fost produsă de executabilul livrat în 0.3.0: 19 credite, tunel 2/3 și lopată în inventar. Importul v4 păstrează aceste valori și creează o ascunzătoare goală; reîncărcarea cu lopata ascunsă trece. Fișierele personale ale utilizatorului nu au fost folosite.

Fereastra Windows Forms a executat deschiderea biroului, comutarea la ascunzătoare, depunerea și recuperarea lopeții. Timpul a rămas oprit în meniu. Sunt comenzi automate într-o fereastră ascunsă, nu apăsări manuale.

## Tunelul și a treia partidă completă

Ruta nouă a fost parcursă cu mișcare normală, obiecte găsite, două lecturi, crafting, trei etape de săpat, așteptarea ceasului și prezență la apelul de seară. Ieșire: ziua 1 la 22:00, fără capturări, gard intact. Doar tranziția finală prin tunel mută jucătorul instantaneu în exterior, conform mecanicii jocului; testul nu acordă obiecte sau timp artificial. Tunelul este o rută fixă, fără explorare subterană.

Testele izolate verifică pragul de intelect, consumul materialelor, distanța, unealta, energia, anularea la mișcare, etapele păstrate după salvare, lipsa recompenselor repetate, intervalul nocturn (inclusiv 00:00 și refuzul la 07:00), blocarea la suspiciune/urmărire, confiscarea și astuparea după capturare, victoria persistentă și respingerea stărilor invalide. Observarea săpatului crește suspiciunea de la zero; un caz separat pornește cu 59 suspiciune și un gardian apropiat pentru a verifica prinderea.

Fișierul sintetic `fixtures/save-v2.xml` a fost serializat de executabilul livrat în 0.2.0, cu 27 credite, un favor activ și două piese lucrate. Migrarea în v3 păstrează aceste valori și inițializează tunelul. Nu s-a modificat salvarea personală a utilizatorului.

Proba ferestrei a executat și comanda meniului pentru lopată, verificând consumul rețetei și obiectul rezultat. Este un apel automat al comenzii, nu o apăsare manuală de tastă.

## Extinderea economică

Au fost verificate programul și plafonul muncii, energia necesară, anularea la mișcare sau la prânz, plata o singură dată, acceptarea/predarea/renunțarea favorurilor, plata zilnică unică, refuzul predării către alt personaj și păstrarea favorului la capturare.

Comerțul a fost verificat pentru bani insuficienți, inventar plin, stoc epuizat, vânzare, refuzul cleștelui, tranzacții de la distanță, suspiciune și urmărire. S-au verificat resetarea zilnică și persistența soldului, stocului, muncii și favorurilor, precum și respingerea datelor economice invalide. Biroul plin nu consumă obiectul și rămâne încărcabil din salvare.

Un al doilea traseu complet pornește din starea inițială, găsește bandă și biscuiți, predă biscuiții lui Matei pentru 18 credite, participă la apel și masă, execută o piesă de atelier pentru 12 credite, cumpără pilă și sârmă pentru 22, studiază, construiește cleștele și evadează cu patrulele active. Rezultat: ziua 1, 10:40, zero capturări, 8 credite rămase. Nu s-au teleportat personaje sau acordat bani artificial în acest traseu; celelalte cazuri izolate folosesc stări pregătite pentru condițiile testate.

Fișierul `fixtures/save-v1.xml` a fost produs cu codul GameModel din livrarea 0.1.0, nu prin redenumirea unei salvări v2. Conține o partidă sintetică cu 40 intelect și bandă în inventar. Importul păstrează progresul și inițializează economia, apoi salvarea rezultată se reîncarcă în v4. Nu s-a citit sau modificat o salvare personală a utilizatorului în aceste teste.

Proba Windows Forms verifică suplimentar că un dialog oprește ceasul și că comenzile ferestrei pentru acceptare/predare procesează un favor. Comenzile sunt apelate în cod, într-o fereastră ascunsă; nu sunt apăsări manuale ale tastelor.

## Comportament verificat

- Poziție inițială validă și acces la fiecare încăpere esențială.
- Coliziuni cu zidurile și gardul; zidurile întrerup vederea gardienilor.
- Șase locuri de inventar; refuzul celui de-al șaptelea obiect.
- Crafting condiționat de intelect și materiale, fără consum la eșec sau duplicare.
- Studiu și tăiere anulate prin mișcare; recuperare la pat.
- Suspiciune la absența de la apel, reducere la prezență, capturare la tăierea observată.
- Confiscare și reaprovizionarea materialelor esențiale după capturare.
- Salvare/încărcare, inclusiv poziția patrulei; respingerea datelor corupte, NaN, recipienților invalizi și pozițiilor în zid.
- Trecerea peste miezul nopții și deplasarea patrulelor fără intrare în ziduri.
- Partidă completă, începută din starea inițială: culegere materiale, prezență la apel și masă, studiu, crafting, așteptarea patrulei, tăiere și trecere prin breșă. Fără teleportarea jucătorului, acordarea obiectelor sau schimbarea ceasului în acest test de integrare. Scenariile unitare izolate folosesc stări pregătite pentru cazurile-limită.
- Bucla reală Windows Forms a pornit, a randat și s-a închis normal. Ceasul a avansat în joc și a rămas nemișcat în pauză. Testul a folosit o fereastră ascunsă, nu interacțiune umană.

Rezultatele exacte și numărul de aserțiuni sunt în `qa/tests.txt` (în surse) sau `dovezi/tests.txt` (în pachet), iar proba ferestrei în `qa/window.txt` / `dovezi/window.txt`. Testul de integrare prin gard evadează în ziua 1, la 10:40, fără capturări.

## Revizie vizuală

Au fost inspectate imaginile generate de rendererul aplicației pentru joc, introducere, crafting, ghid și victorie în 0.1, apoi dialogul de favor și magazinul în 0.2, meniul cu două unelte și starea tunelului în 0.3, ascunzătoarea plină și ultima pagină a biroului în 0.4. Textele, prețurile, stocurile și butoanele noilor ecrane încap în panourile de 1280×900. Starea fără fonduri sau fără obiecte este afișată cu butoane inactive. Fereastra scalează compoziția păstrând proporțiile și oferă F11 pentru ecran complet.

Nu s-au identificat suprapuneri critice în aceste ecrane. La ferestre mici textul se micșorează; ecranul complet este preferabil pentru lizibilitate. Etichetele hărții și marcajele personajelor rămân simplificate, potrivite unui prototip.

## Limite ale verificării

Capturile sunt randări ale aceleiași interfețe folosite în aplicație, nu fotografii ale unei sesiuni jucate manual. Nu s-au verificat manual toate combinațiile tastatură/mouse, toate setările DPI, screen readers sau Windows 10/11 pe calculatoare diferite. Testul de performanță din jurnal măsoară media simulare + randare offscreen pe 100 de cadre; nu certifică FPS-ul afișajului sau comportamentul pe hardware diferit.

## Proveniență

Referință de proiectare: `The_Escapists_documentatie_consolidata_2026-10-13.zip`, SHA-256 `4ccc979c0042121cf1943aeb5cff5448fd70fb87e4da52e8ca70a2ed06ef060f`. Specificația documentează mai ales editorul și formatele de hărți; mecanicile și cifrele acestei versiuni sunt ale prototipului, nu rezultate verificate despre motorul comercial. Nu s-a preluat cod upstream sau grafică comercială.
