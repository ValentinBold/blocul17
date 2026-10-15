# Plan 0.6 — Securitate și contrabandă

## Scop

Creșterea rolului planificării: transportă materialele în timpul atelierului, ascunde rezervele înainte de percheziție și recuperează obiectele confiscate folosind economia existentă. Regulile sunt originale, nu o afirmație de paritate cu The Escapists.

## Pachet de implementare

1. Contrabandă explicită: pilă, sârmă, clește și lopată, marcate în inventar și recipiente.
2. Detector la ușa atelierului: +35 suspiciune la intrarea pe marcaj cu obiecte interzise; pilă și sârmă sunt exceptate între 09:00 și 12:00. Uneltele construite nu sunt exceptate. Staționarea nu repetă alarma; o traversare nouă poate declanșa alta.
3. Percheziție a biroului zilnic la 18:30, anunțată la 18:00. Ascunzătoarea și buzunarele nu sunt inspectate de acest eveniment. Descoperirea contrabandei adaugă 20 suspiciune.
4. Depozit de confiscări persistent. Un ghișeu lângă cantină permite recuperarea contra cost între 12:00 și 22:00. Capturarea transferă și contrabanda din buzunare și birou în depozit; restul consecințelor rămân.
5. Panou K pentru securitate, semnalizare pe hartă, meniu de recuperare, ghid și jurnal actualizate.
6. Salvare v5 separată cu import v1–v4, fără modificarea originalelor. Evenimentele sunt procesate la trecerea pragului, fără percheziții retroactive la import.

## Criterii de acceptare

- Fără confiscare a ascunzătorii, recompense sau tranzacții duplicate după încărcare.
- Recuperarea refuzată nu consumă credite sau obiecte; distanța, ora, capacitatea și urmărirea sunt verificate.
- Așteptarea accelerată se oprește la anunț/control; nu sare peste evenimente.
- Traseele existente rămân realizabile. Un scenariu nou acoperă natural depozitare, muncă, percheziție, recuperare și evadare.
- Teste pentru intervale, salvări invalide, zile multiple, capturare, UI și pachetul distribuit.
- Revizie vizuală a noilor ecrane, CI Windows, Pull Request și release 0.6.0 cu verificare SHA-256.

## Limite intenționate

Controlul biroului este un eveniment determinist, fără animație de gardian care caută în sertare. Ghișeul este o regulă de recuperare a prototipului, nu un sistem juridic. Nu adăugăm combat, multiplayer sau un editor în acest pachet.

## Rezultat implementat

Mecanicile și interfața sunt implementate. 217 aserțiuni trec local, inclusiv cinci trasee complete; ruta nouă se încheie la 20:18, ziua 1, cu zero capturări. Proba Windows Forms verifică recuperarea și pauza în meniuri. Detaliile și limitele verificării sunt în [VERIFICARE.md](VERIFICARE.md).
