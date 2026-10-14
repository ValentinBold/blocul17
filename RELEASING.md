# Actualizări și lansări

`main` conține modificările integrate. O lansare este identificată printr-un tag de forma `v0.4.0`. Patch-urile pot folosi `0.4.1`, iar extinderile `0.5.0`. Nu muta tag-uri deja publicate.

## Pregătire

1. Lucrează pe o ramură și deschide un Pull Request.
2. Actualizează `AssemblyInfo.cs` (cele două versiuni) și `app.manifest` la aceeași versiune, de exemplu `0.5.0.0`.
3. Actualizează `CHANGELOG.md`, `README.md`, `CITESTE_MA.txt` și `VERIFICARE.md`. Nu declara teste care nu au fost rulate.
4. Rulează `test.ps1` și verifică interfața. Integrează Pull Request-ul după verificarea automată **Windows tests**.

## Publicare

Din copia locală actualizată a ramurii `main`:

```powershell
git pull --ff-only
git tag -a v0.5.0 -m "Blocul 17 v0.5.0"
git push origin v0.5.0
```

Înlocuiește versiunea din exemplu cu cea pregătită. Workflow-ul **Release** verifică potrivirea dintre tag, assembly și manifest, rulează testele și generează arhiva Windows cu SHA-256. Creează mai întâi un draft, încarcă fișierele și apoi publică release-ul. Notele provin din secțiunea versiunii din `CHANGELOG.md`.

Dacă un run eșuează, verifică jurnalul din Actions înainte să reîncerci. Un draft poate fi completat prin reluarea workflow-ului; o versiune deja publicată nu este suprascrisă. Pentru o corecție după publicare, folosește un tag nou.

## GitHub Desktop

Adaugă copia locală prin **File → Add local repository**, alege ramura pentru modificări, apoi folosește **Commit**, **Push origin** și **Create Pull Request**. Tag-ul de lansare poate fi creat prin comenzile de mai sus, din **Repository → Open in terminal**.

Jocul nu descarcă singur actualizările. Utilizatorii iau ZIP-ul nou din Releases; progresul local se păstrează/importă conform versiunii salvării.

Referințe: [workflow permissions](https://docs.github.com/en/actions/reference/workflows-and-actions/workflow-syntax#permissions), [GitHub CLI release create](https://cli.github.com/manual/gh_release_create).
