# Blocul 17 — prototip de evadare

Sursă de orientare: The_Escapists_documentatie_consolidata_2026-10-13.zip, SHA-256 4ccc979c0042121cf1943aeb5cff5448fd70fb87e4da52e8ca70a2ed06ef060f.

Acesta este un joc original inspirat de bucla de evadare din The Escapists. Nu importă hărți comerciale, resurse grafice sau binare J2ME și nu promite paritate cu motorul original. Arhiva este referință, nu instrucțiuni executabile.

## Prima felie jucabilă

- O închisoare cu celule, cantină, curte, bibliotecă și atelier. Recuperarea sănătății se face în pat.
- Mișcare cu coliziuni, interacțiuni la distanță mică și șase locuri de inventar.
- Rutine zilnice; apelul și masa cer prezența într-o zonă, cu perioadă de grație.
- Gardieni cu patrule, linie de vedere și urmărire prin navigare în jurul obstacolelor.
- Materiale în containere, rețetă de clește, studiu, hrană și odihnă.
- Tăierea gardului necesită clește, energie și timp; traversarea breșei produce victoria.
- Prinderea confiscă obiectele, resetează materialele necesare și permite continuarea.
- Pauză, instrucțiuni și salvare locală versionată.

## Alegeri proprii, nu fapte verificate despre The Escapists

Harta, personajele, numele, programul accelerat, rețetele, valorile statisticilor, conurile de vedere, consecințele prinderii și ruta de evadare sunt proiectate pentru această versiune. Documentația verificată nu furnizează specificația completă pentru acestea.

## Criterii de acceptare

Jucătorul poate încheia o partidă fără comenzi ascunse. Nu poate traversa pereți sau gardul intact. Crafting-ul consumă exact materialele și respectă capacitatea inventarului. Gardienii nu văd prin ziduri. Salvările invalide nu blochează pornirea. Progresul și timpul stau pe loc în pauză și când fereastra nu este activă.


## Extindere 0.2: economie și relații

Munca, favorurile și comerțul constituie o cale alternativă de procurare a materialelor. Ruta prin recipiente rămâne disponibilă. Recompensele, personajele și prețurile sunt reguli proprii prototipului.

Un favor nu plătește de două ori în aceeași zi. Operațiile de comerț sunt indivizibile: o eroare nu modifică banii, inventarul sau stocul. Capturarea nu resetează plafonul de muncă sau istoricul favorurilor. Dialogurile se deschid numai la distanță mică și în afara unei urmăriri; simularea este oprită pe durata lor.

Salvarea v2 păstrează economia și migrează v1 cu valori inițiale explicite. Fișierul vechi nu este suprascris. Echilibrarea pe multe zile și repetarea intenționată a capturărilor pentru reaprovizionare rămân de evaluat; probele complete acoperă două trasee în prima zi.

## Extindere 0.3: ruta prin tunel

O alternativă fixă la gard: o lopată folosește același set de materiale, dar cere 40 intelect. Trei acțiuni de 8 secunde consumă câte 20 energie și păstrează progresul între sesiuni. Ieșirea nocturnă cere suspiciune sub 60 și lipsa unei urmăriri. Capturarea astupă tunelul. Traversarea este o tranziție instantanee, fără nivel subteran sau săpare liberă.

Valorile sunt ale prototipului. Ruta se poate pregăti în bibliotecă fără a intersecta patrula obișnuită; un gardian atras acolo vede și sancționează săpatul. Testul complet respectă apelul de seară, apoi așteaptă noaptea. Ritmul acestei așteptări și dificultatea comparată cu gardul rămân de evaluat prin joc manual. Salvarea v3 păstrează și economia din v2.

## Extindere 0.4: inventar și rezervă protejată

Recipientele se deschid într-un meniu cu timpul oprit. Transferurile verifică distanța, vederea, capacitatea și starea jucătorului. Depozitarea se face numai lângă biroul propriu. Cele trei locuri ascunse permit păstrarea unei rețete sau a uneltelor după capturare; sertarul obișnuit rămâne neprotejat. Partida nouă golește ascunzătoarea.

Aceasta este o regulă proprie prototipului, fără sistem de percheziții aleatorii. Salvarea v4 migrează formatele vechi și păstrează economia și tunelul. Nu există transfer de obiecte între partide. Efectul ascunzătorii asupra dificultății unei campanii cu multe capturări rămâne de evaluat manual.

## Extindere 0.5: așteptare accelerată

Așteptarea rulează maximum 20 de pași normali per cadru, fără salt direct al ceasului. Se oprește la limita rutinei, la schimbarea zilei sau când nu mai sunt îndeplinite condițiile de siguranță. Prezența la program este obligatorie chiar și în perioada de grație. Pragul de suspiciune este 25. Nu accelerează acțiunile de studiu, muncă sau săpat.

Starea Waiting este tranzitorie, în afara SaveData. Formatul de salvare v4 rămâne compatibil. Rata afișată ×20 este multiplicatorul pașilor de simulare, nu o garanție de performanță pe orice hardware.

## Extindere 0.6: securitate și contrabandă

Detectorul de la ușa atelierului introduce o decizie de transport: materialele brute au permis 09:00–12:00; uneltele construite declanșează alarma oricând. +35 suspiciune se aplică la intrarea pe marcaj, nu în fiecare cadru. Urmărirea folosește gardienii existenți, fără confiscare instantanee la detector.

Biroul este controlat la 18:30, cu anunț la 18:00. Sunt inspectate numai obiectele din sertarul obișnuit. Ascunzătoarea și buzunarele sunt excluse din acest eveniment. Controlul nu are animație de gardian, iar regulile nu sunt aleatorii. Descoperirea contrabandei adaugă 20 suspiciune o singură dată. Timpul accelerat se oprește la eveniment.

Contrabanda confiscată la control sau capturare este contabilizată separat pe patru tipuri. Recuperarea la ghișeul de lângă cantină cere orele 12:00–22:00, apropiere, lipsa urmăririi și spațiu în inventar. Prețuri: pilă 8, sârmă 6, clește 24, lopată 28. Plata și transferul sunt atomice. Plafonul defensiv este un milion pe tip; surplusul nu este șters, nici la capturare.

Salvarea v5 persistă depozitul și zilele controlului/anunțului. Importul v1–v4 inițializează securitatea și păstrează progresul. Pragurile se aplică numai când ceasul le traversează; nu se execută control retroactiv la încărcare. Panoul K și ghișeul opresc timpul. Valorile de echilibrare sunt ale prototipului și rămân de evaluat prin joc manual.

## Extindere 0.7: planificare

Panoul P separă gardul, tunelul și programul zilnic. Prezintă stocurile proprii pe patru locații și recomandă un pas contextual, fără transferuri automate, consum de resurse sau modificarea salvării. Rutele nu sunt exclusive. Victor preferă sfatul pentru tunel când există progres de săpare sau o lopată deținută, altfel sfatul pentru gard.

Priorități: stare finală, urmărire/suspiciune, ieșire deja pregătită, rutină, acțiune în curs, control iminent, unealtă deținută, intelect și rețetă. Alegerea unei file este doar de interfață; formatul v5 rămâne identic cu 0.6.
