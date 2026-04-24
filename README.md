# Analyse des besoins — Gestion_RDV

## Contexte

Petit cabinet médical gérant des patients, des médecins et des rendez-vous.
Actuellement tout est géré à la main (papier, Excel, agenda…).

---

## Ce qui existe déjà dans l'app

| Module | Ce qui est fait |
|---|---|
| **Patients** | Fiche complète (nom, email, téléphone, date de naissance, notes médicales, âge calculé automatiquement) |
| **Médecins** | Fiche (nom, spécialité, email, téléphone) |
| **Rendez-vous** | Création d'un RDV lié à un patient + un médecin, avec type (Consultation / Contrôle / Urgence), statut (Normal / Urgent / Annulé), date/heure et notes |
| **Dashboard** | Vue du jour : compteurs (total patients, médecins, RDV du jour, urgences), liste des RDV du jour triés par heure |
| **Anti-collision** | Empêche de créer 2 RDV au même créneau pour le même médecin |
| **Base locale** | SQLite embarqué, données stockées sur l'appareil, pas besoin d'internet |
| **Multi-plateforme** | Tourne sur Windows, Android, iOS |

---

## Ce qui manque / ce dont ils ont besoin

### 1. Recherche et filtres
**Besoin :** trouver rapidement un patient ou un médecin dans une longue liste.
**Problème actuel :** les listes sont brutes, il faut scroller pour trouver quelqu'un.

### 2. Notifications / rappels
**Besoin :** alerter le secrétaire ou le patient avant un RDV.
**Problème actuel :** aucun rappel automatique, risque d'oubli ou de no-show.

### 3. Vue agenda / calendrier
**Besoin :** visualiser les RDV sous forme de calendrier (semaine / mois).
**Problème actuel :** le dashboard ne montre que le jour J, impossible de voir la semaine d'un coup.

### 4. Gestion des créneaux horaires du médecin
**Besoin :** définir les horaires de disponibilité de chaque médecin.
**Problème actuel :** on peut créer un RDV à n'importe quelle heure, même la nuit ou le weekend.

### 5. Historique des RDV par patient
**Besoin :** voir tous les anciens RDV d'un patient depuis sa fiche.
**Problème actuel :** aucun lien entre la fiche patient et ses rendez-vous passés.

### 6. Export / impression
**Besoin :** exporter la liste des RDV du jour en PDF ou imprimer un planning.
**Problème actuel :** tout reste dans l'app, pas de sortie papier ou numérique.

### 7. Gestion des annulations
**Besoin :** annuler un RDV avec un motif, et libérer le créneau.
**Problème actuel :** le statut "Annulé" existe mais le créneau n'est pas libéré dans la logique anti-collision.

---

## Ce que l'app résout déjà

- **Fini les doubles réservations** : l'anti-collision bloque automatiquement deux RDV au même créneau chez le même médecin.
- **Tout centralisé** : patients, médecins et RDV au même endroit, plus de fichiers Excel éparpillés.
- **Vue du jour immédiate** : le secrétaire voit en un coup d'œil les urgences et l'agenda du jour.
- **Fonctionne sans internet** : données stockées localement, utilisable n'importe où.
- **Portable** : Windows au cabinet, Android en déplacement, même app.

---

## Priorités suggérées pour la suite

| Priorité | Feature | Effort estimé |
|---|---|---|
| 🔴 Haute | Recherche dans les listes | Faible |
| 🔴 Haute | Historique RDV par patient | Faible |
| 🟠 Moyenne | Vue calendrier (semaine) | Moyen |
| 🟠 Moyenne | Fix annulation + libération créneau | Faible |
| 🟡 Basse | Notifications/rappels | Moyen |
| 🟡 Basse | Export PDF | Moyen |
| 🟡 Basse | Créneaux de disponibilité médecin | Élevé |
