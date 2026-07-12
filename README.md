# BoxAverageTracker

Tracker de box hits pour Black Ops 1 Zombies. Lit la mémoire du jeu en temps réel pour compter automatiquement les hits de boîte et suivre les trades.

---

## Installation

1. Télécharge et extrais le dossier
2. Lance `BoxAverageTracker.exe`
3. Lance ta partie — le tracker se connecte automatiquement au jeu

---

## Utilisation

Fais un **clic droit** sur la fenêtre pour accéder au menu.

**Options principales :**
- **Setup finished** — à cocher une fois que le setup de ta game est terminé, les trades et averages commencent à être comptés
- **Auto finish setup** — se coche automatiquement 1 heure après le dernier restart si tu oublies
- **Auto display weapons** — affiche automatiquement les armes à trader selon la map

**Affichage :**
- **Current box hits** — affiche le compteur de hits depuis le dernier teddy
- **Current trade hits** — affiche le nombre de hits depuis le début du trade en cours
- **Total trades** — affiche le nombre de trades effectués
- **Worst trades / Best trades** — affiche les pires et meilleurs trades de la game, avec un slider pour choisir combien en afficher

**Sons :**
- **Sounds enabled** — active ou désactive les sons de trade
- Les sons se jouent quand tu récupères une arme cochée dans **Trading**
- Place tes fichiers `.wav` dans les dossiers correspondants dans `sounds/`

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

Tu peux mettre autant de `.wav` que tu veux dans chaque dossier, un sera choisi au hasard.

---

## Maps supportées

Ascension, Call of the Dead, Der Riese, Five, Kino der Toten, Moon, Nacht Der Untoten, Shangri-La, Shi No Numa, Verrückt

---

## Sauvegarde

- **Save game** — sauvegarde la game en cours dans un fichier `.txt`
- **Load game** — charge une game sauvegardée
- **Export stats** — exporte les stats dans un fichier `.txt`
