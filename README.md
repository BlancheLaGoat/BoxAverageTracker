# BoxAverageTracker

Mystery box tracker for **Black Ops 1 Zombies**: box hits, trade hits, per-weapon averages, best/worst trades, custom sounds.

*🇫🇷 Version française plus bas.*

---

## Installation

Download **`BoxAverageTracker.exe`** from the [latest release](https://github.com/BlancheLaGoat/BoxAverageTracker/releases/tag/latest) and run it. That's it — a single file, you can put it anywhere (even on your desktop).

Everything the app creates (sounds, settings, saved games) lives in **`%AppData%\BoxAverageTracker`**, never next to the exe. Right-click → **Open data folder...** to get there.

---

## Usage

**Right-click** the window to open the menu.

**Main options:**
- **Setup finished** — tick it once your game setup is done; trades and averages start being counted
- **Auto finish setup** — ticks itself 1 hour after the last restart if you forget
- **Auto display weapons** — automatically shows the weapons to trade for the current map

**Display:**
- **Current box hits** — hit counter since the last teddy bear
- **Current trade hits** — hits since the start of the current trade
- **Total trades** — number of trades done
- **Worst trades / Best trades** — worst and best trades of the game, with a slider to choose how many to show
- **Weapon popup** — when you get a weapon from the box (or a teddy bear), shows for a few seconds under the averages: `China Lake  avg 12.50  (x3)` (that weapon's average + how many times you got it). Duration can be changed with `WeaponPopupSeconds=` in `ApplicationConfig.txt` (4 s by default)

**Sounds:**
- **Sounds enabled** — turns sounds on or off
- **Convert sounds...** — converts your audio files to the right format and puts them in the right folder (see below)

---

## Sounds

All sound folders are **created automatically (empty)** in `%AppData%\BoxAverageTracker\sounds`. Put `.wav` files in them; if a folder has several, one is picked at random.

### Per-weapon sounds

Each box weapon has its own folder. When you get that weapon, its sound plays **instead of** the trade sound.

```
sounds/
  thundergun/     → Thundergun
  raygun/         → Ray Gun
  wunderwaffedg2/ → Wunderwaffe DG-2
  wintershowl/    → Winter's howl
  3179jgb215/     → 31-79 Jgb 215 (Baby gun)
  chinalake/      → China Lake
  gersh/  qed/  wavegun/  scavenger/  vr11/  m72law/ ...
```

### Trade sounds

Played when you get a weapon ticked in **Trading...**, depending on how many hits the trade took (only if that weapon has no sound of its own):

```
sounds/
  trade_1/       → 1st hit
  trade_2-10/    → 2 to 10 hits
  trade_10-20/   → 11 to 20 hits
  trade_20-30/   → 21 to 30 hits
  trade_30-50/   → 31 to 50 hits
  trade_50-100/  → 51 to 100 hits
  trade_100+/    → 101+ hits
```

### Converting sounds

Right-click → **Convert sounds...**: pick the weapon (or a trade folder), then drop your files into the window (or use **Choose files...**). They are converted to a compatible `.wav` and saved straight into the right folder.

Accepted formats: anything Windows can play — mp3, mp4, m4a, aac, wav, wma, mov... (videos work too, only the audio is kept).

---

## Automatic updates

On launch, the app checks the GitHub **latest** release. If a newer version exists, it offers to download it, replaces the exe and restarts by itself. Your sounds and settings are not touched.

- **Check for updates...** in the right-click menu to check manually
- **About** shows the current build number
- An exe built locally in Visual Studio (build 0) never updates itself

---

## Saving

- **Save game** — saves the current game to a `.txt` file in `%AppData%\BoxAverageTracker\games`
- **Load game** — loads a saved game
- **Export stats** — exports the stats to a `.txt` file (same folder) and opens it

---

## Supported maps

Ascension, Call of the Dead, Der Riese, Five, Kino der Toten, Moon, Nacht Der Untoten, Shangri-La, Shi No Numa, Verrückt

---
---

# 🇫🇷 Version française

## Installation

Téléchargez **`BoxAverageTracker.exe`** depuis la [dernière release](https://github.com/BlancheLaGoat/BoxAverageTracker/releases/tag/latest) et lancez-le. C'est tout : un seul fichier, à mettre où vous voulez (même sur le bureau).

Tout ce que l'appli crée (sons, réglages, parties sauvegardées) est rangé dans **`%AppData%\BoxAverageTracker`**, jamais à côté de l'exe. Clic droit → **Open data folder...** pour y accéder.

## Utilisation

**Clic droit** sur la fenêtre pour ouvrir le menu.

- **Setup finished** — à cocher une fois le setup terminé ; les trades et averages commencent à être comptés
- **Auto finish setup** — se coche tout seul 1 h après le dernier restart si vous oubliez
- **Auto display weapons** — affiche automatiquement les armes à trader selon la map
- **Current box hits / Current trade hits / Total trades** — compteurs affichés
- **Worst trades / Best trades** — pires et meilleurs trades de la game, avec un slider pour le nombre affiché
- **Weapon popup** — quand vous choppez une arme (ou un teddy), affiche quelques secondes : `China Lake  avg 12.50  (x3)`. Durée réglable avec `WeaponPopupSeconds=` dans `ApplicationConfig.txt`

## Sons

Tous les dossiers de sons sont **créés automatiquement (vides)** dans `%AppData%\BoxAverageTracker\sounds` : un par arme de la box (`thundergun`, `raygun`, `chinalake`...) et un par tranche de trade (`trade_1` à `trade_100+`). Mettez-y des `.wav` ; s'il y en a plusieurs, un est tiré au hasard. Le son d'une arme passe **avant** le son de trade.

Clic droit → **Convert sounds...** : choisissez l'arme, glissez vos fichiers (mp3, mp4, m4a, wav...) dans la fenêtre, ils sont convertis en `.wav` et rangés dans le bon dossier.

## Mises à jour automatiques

Au lancement, l'appli vérifie la release GitHub **latest** et propose la mise à jour s'il y en a une ; elle remplace l'exe et se relance. Vos sons et réglages ne sont pas touchés. **Check for updates...** pour vérifier à la main, **About** pour voir le numéro de build.

## Sauvegarde

**Save game**, **Load game** et **Export stats** utilisent le dossier `%AppData%\BoxAverageTracker\games`.
