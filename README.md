# BoxAverageTracker

---

## Installation

Prenez soit le .exe si vous ne voulez pas les sons, sois vous voulez les sons, alors vous prenez le .zip puis suivez ce chemin "BoxAverageTracker-latest\bin\Release" pour avoir le dossier "sounds". 

---

## Utilisation

Faites un **clic droit** sur la fenêtre pour accéder au menu.

**Options principales :**
- **Setup finished** — à cocher une fois que le setup de ta game est terminé, les trades et averages commencent à être comptés
- **Auto finish setup** — se coche automatiquement 1 heure après le dernier restart si tu oublies
- **Auto display weapons** — affiche automatiquement les armes à trader selon la map

**Affichage :**
- **Current box hits** — affiche le compteur de hits depuis le dernier teddy
- **Current trade hits** — affiche le nombre de hits depuis le début du trade en cours
- **Total trades** — affiche le nombre de trades effectués
- **Worst trades / Best trades** — affiche les pires et meilleurs trades de la game, avec un slider pour choisir combien en afficher
- **Weapon popup** — quand tu choppes une arme dans la box (ou un teddy), affiche quelques secondes sous les averages : `China Lake  avg 12.50  (x3)` (average de l'arme + nombre de fois obtenue). Durée réglable via `WeaponPopupSeconds=` dans `ApplicationConfig.txt` (4 s par défaut)

**Sons :**
- **Sounds enabled** — active ou désactive les sons de trade
- Les sons se jouent quand vous récupérez une arme cochée dans **Trading**
- Placez vos fichiers `.wav` dans les dossiers correspondants dans `sounds/`

---

## Mises à jour automatiques

Au lancement, l'appli vérifie s'il existe une version plus récente sur la Release GitHub **latest**. Si oui, elle te propose de la télécharger, remplace l'exe et se relance toute seule (tes sons et ton `ApplicationConfig.txt` ne sont pas touchés).

- **Check for updates...** dans le menu clic droit pour vérifier à la main
- **About** affiche le numéro de build actuel
- Un exe compilé en local dans Visual Studio (build 0) ne se met jamais à jour tout seul

---

## Convertir des sons

Clic droit → **Convert sounds...** : choisis l'arme (ou un dossier de trade), puis glisse tes fichiers dans la fenêtre (ou « Choisir des fichiers... »). Ils sont convertis en `.wav` compatible et rangés directement dans le bon dossier.

Formats acceptés : tout ce que Windows sait lire — mp3, mp4, m4a, aac, wav, wma, mov... (une vidéo est acceptée, seul le son est gardé).

---

## Dossiers de sons

Les sons sont dans `sounds/` à côté de l'exe, organisés par nombre de trade hits :

```
sounds/
  trade_1/       → 1er hit
  trade_2-10/    → 2 à 10 hits
  trade_10-20/   → 11 à 20 hits
  trade_20-30/   → 21 à 30 hits
  trade_30-50/   → 31 à 50 hits
  trade_50-100/  → 51 à 100 hits
  trade_100+/    → 101+ hits
```

Vous pouvez mettre autant de `.wav` que tu veux dans chaque dossier, un sera choisi au hasard.

### Sons par arme

Tu peux aussi donner un son à une arme précise : mets des `.wav` dans le dossier `sounds/<nom de l'arme>/` (en minuscules, sans espaces ni caractères spéciaux). Tous les dossiers (armes de la box + trades) sont **créés automatiquement** au lancement de l'appli. Quand tu choppes cette arme, son son passe **en priorité** sur le son de trade.

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

---

## Maps supportées

Ascension, Call of the Dead, Der Riese, Five, Kino der Toten, Moon, Nacht Der Untoten, Shangri-La, Shi No Numa, Verrückt

---

## Sauvegarde

- **Save game** — sauvegarde la game en cours dans un fichier `.txt`
- **Load game** — charge une game sauvegardée
- **Export stats** — exporte les stats dans un fichier `.txt`
