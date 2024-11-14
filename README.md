# SE 24 Projektas

Projekto tikslas sukurti aplikaciją, kuri žaidimais padėtų lavinti kognityvinius įgūdžius.

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

## Duomenų bazė

Naudojama PostgreSQL duomenų bazė.
Kiekvienam programuotojui reikia susikūrti duomenų bazę ant savo kompiuterio asmeniškai.
Pakeisti duomenų bazės pavadinimą, vartotojo ID, slaptažodį tarp appsettings.json -> ConnectionStrings -> DefaultConnection 
į atitinkamus duomenis, kuriuos naudojote kurdami duomenų bazę ant savo kompiuterio. Taip pat bus reikalingas įrankis
norint dirbti su migracijomis, kurį galima gauti į terminalą suvedus komandą `dotnet tool install --global dotnet-ef`.

### Naudingos komandos

` dotnet ef database update` - atnaujina duomenų bazę naudojant migracijas.
` dotnet ef migrations add <migracijos_pavadinimas>` - sukuria naują migraciją nurodytu pavadinimu.