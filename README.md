# 🎮 Bomberman - WPF MVVM

---

## 🧩 Introduktion

Vår version av **Bomberman** är baserad på det klassiska spelet där spelaren placerar bomber för att förstöra hinder och samla poäng.  
I vår twist spelar man mot sig själv i ett oändligt spel med genererade banor och en ständigt ökande svårighetsgrad, där slumpmässigt genererade bomber ökar i antal i takt med spelets progression.  
Målet är att samla så många poäng som möjligt för att ta sig in på topplistan över spelare.  
Programmet är utvecklat i **C#** med hjälp av **WPF** och följer **MVVM-arkitekturen**.

---

## 🎯 Syfte med projektet

- Lära sig och implementera och arbeta efter **MVVM-arkitekturen**.  
- Utforska spelutveckling med **WPF**.

---

## 🧱 Funktioner

- 💣 **Placerbara bomber:** Spräng hinder.  
- 💰 **Poäng-mynt:** Plocka upp mynt för att öka poängen.  
- 🗺️ **Genererade banor:** Spela på oändligt många nivåer som genereras dynamiskt.  
- 🔀 **Genererade bomber:** Slumpvist genererade bomber som ökar i antal baserat på spelets progression.  
- ⚡ **Booster-system:** Samla boosters för att öka bombkraft och rörelsehastighet.  
- 🎵 **Ljudeffekter och musik:** Förhöjer spelupplevelsen och ger en mer immersiv atmosfär.  
- ☠️ **Game Over-skärm:** När spelaren dör visas en Game Over-skärm med möjlighet att starta om och se topplistan över spelare.

---

## 🛠️ Teknologier och Verktyg

| Typ | Verktyg |
|-----|----------|
| **Språk** | C# |
| **IDE** | Visual Studio |
| **Ramverk** | Windows Presentation Foundation (WPF) |
| **Arkitektur** | MVVM (Model-View-ViewModel) |
| **Andra verktyg** | NuGet-paket, Behaviors, xUnit, DispatcherTimer |

---

## 🧠 Arkitektur och MVVM

Spelet är uppbyggt enligt **MVVM-arkitekturen** för att separera logik och gränssnitt:

- **Model:** Hanterar speldata som banor, spelobjekt och regler.  
- **View:** Ansvarar för gränssnittet och designen, byggt med XAML.  
- **ViewModel:** Innehåller bindningar och spelmekanik som styr View.

---

## 🚀 Installation och Uppstart

### Krav:
- Visual Studio (med stöd för WPF)  
- .NET Framework 4.7 eller högre  

### Installationssteg:

1. Klona repot:
   ```bash
   git clone https://github.com/lohrberg/Bomberman.git
   ```
2. Öppna lösningen i Visual Studio.  
3. Bygg och kör projektet genom att trycka på **F5**.

---

## 🎮 Kontroller

| Tangent | Funktion |
|----------|-----------|
| ⬆️⬇️⬅️➡️ | Röra spelaren |
| ␣ (Mellanslag) | Placera bomber |
| ❓ Hjälp-knapp | Visa instruktioner och kort förklaring |

---

## 🕹️ Spelmekanik

- **Mål:** Spräng hinder och undvik bomber för att samla poäng och överleva så länge som möjligt.  
- **Bomber:** Genereras slumpvis och måste undvikas av spelaren.  
- **Boosters:** Ger fördelar som ökad rörelsehastighet och bombstyrka.  
- **Game Over:** Om spelaren träffas av en bomb avslutas spelet.

---

## 🧪 Tester

Vi har skrivit enhetstester för att säkerställa spelets funktionalitet och kvalitet.  
Testerna är implementerade med hjälp av **xUnit** och täcker spelmekanik, bombinteraktion och navigering.

---

## 🚧 Framtida Utveckling

- Implementera **multiplayer-läge** för konkurrens mellan spelare.  
- Utöka **nivågenerationen** med fler variationer och tematiska miljöer.  
- Publicera projektet på en server för att möjliggöra **online-åtkomst** och spel via webben.

---

## 👥 Projektteam

| Namn | Roll | GitHub |
|-------|------|--------|
| **Liam Örberg** | Utvecklare | [lohrberg](https://github.com/lohrberg) |
| **Markus Olsson** | Utvecklare | [MarkusOlsson123](https://github.com/MarkusOlsson123) |
| **Johan Andersson** | Utvecklare | [JohanAnderssonOSD](https://github.com/JohanAnderssonOSD) |
| **Christine Lundberg** | Utvecklare | _(GitHub-länk saknas)_ |

---

🧨 *Tack för att du kollar in vårt projekt!* 💥
