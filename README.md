# SE 24 Projektas

Projekto tikslas sukurti aplikaciją, kuri žaidimais padėtų lavinti kognityvinius įgūdžius.

## Setup

Reikalinga PostgreSQL duomenų bazė. Kuriant duomenų bazė peržiūrėti `appsettings.json` failą tarp `se-24.backend`, kad žinoti ką suvesti.

Norint dirbti su migracijomis reikalingas `dotnet ef` įrankis, kurį galime gauti panaudojus komandą: `dotnet tool install --global dotnet-ef`.

Norint sugeneruoti code coverage ataskaitą reikalingas `reportgenerator` įrankis, kurį galime gauti panaudojus komandą: `dotnet tool install -g dotnet-reportgenerator-globaltool`.

## Naudingos komandos

- `dotnet ef migrations add <migracijos_pavadinimas> --project se-24.backend` - sukuria migraciją tarp `Migrations` aplanko.
- `dotnet ef database update --project se-24.backend` - atnaujina duomenų bazę pagal migracijas.
- `dotnet test` - paleidžia testus.
- `dotnet test --collect:"XPlat Code Coverage" --settings se-24.tests/coverlet.runsettings` - paleidžia testus su coverage collection.
- `reportgenerator "-reports:se-24.tests/TestResults/**/*.xml" "-targetdir:se-24.tests/coverage-report" -reporttypes:Html` - sugeneruoja code coverage ataskaitą.

## Funkcionalumas

### 1. **Vartotojo autentifikavimas**
   - **Aprašymas**: Vartotojo paskyros susikūrimas, prisijungimas verifikacija.
   - **Key Features**:
     - Vartotojo prisijungimas atsijungimas
     - Slaptažodžio nuresetinimas
     - Pašto patvirtinimas


### 2. **Žaidimai**
   - **Aprašymas**: Žaidimai suskirstyti į 2-3 kategorijas (Atminties lavinimo, pastabumo) pagal lavinamus įgūdžius. Kiekvieną kategoriją sudarys 2-3 skirtingi žaidimai.
   - **Key Features**:
     - Kategorija nurodanti kokį įgūdį vartotojas nori tobulinti
     - Žaidimo pasirinkimas
     - Tęstinumas vartotojui prisijungus kitą dieną

### 2.1 **Kategorijos**
   - **Atminties lavinimo**:
     - Greito skaitymo žaidimas
     - Užsidegančių kvadratelių suspaudymas ta pačia tvarka kuria jie pasirodė
     
   - **Pastabumo**:
     - Skirtumų radimo žaidimas (Time based)
     - Where's Waldo stiliaus žaidimas (Time based)
     

### 3. **Žaidėjų leaderboard'as**
   - **Aprašymas**: Žaidėjai atlikdami užduotis žaidimų metu gauna taškus kurie yra sudedami ir pagal šiuos taškus žaidėjai reitinguojami lentelėje
   - **Key Features**:
     - Taškų surinkimas
     - Taškų sudėjimas
     - Palyginimas su kitais lygoje esančiais žaidėjais lentelėje

### 4. **Taškų skaičiavimas**
   - **Aprašymas**: Kodo dalis skirta pagal sukurtas formules konvertuoti žaidėjo pasiekimus (jei užduoties atlikimas matuojamas laiku) konvertuoti į taškus.
   - **Key Features**:
     - Taškų apskaičiavimas
