# 📅 Gestion des Conflits de Rendez-vous

## Vue d'ensemble

Le système de gestion des rendez-vous implémente une vérification stricte des conflits pour éviter les chevauchements et garantir un espacement minimal entre les rendez-vous.

## Règles de Validation

### 1. **Intervalle Minimal de 10 Minutes**
- Chaque rendez-vous doit avoir au moins **10 minutes d'écart** avec les autres rendez-vous
- Cette règle s'applique **avant** et **après** le rendez-vous
- Exemple : Si un RDV est prévu à 10:00, aucun autre RDV ne peut être créé entre 9:50 et 10:10

### 2. **Vérification pour le Médecin**
Le système vérifie que le médecin n'a pas déjà de rendez-vous :
- À l'heure exacte demandée
- Dans l'intervalle de ±10 minutes

**Message d'erreur :**
```
Le Dr. [Nom] a déjà un rendez-vous avec [Patient] à [Heure]

Il doit y avoir au moins 10 minutes d'intervalle entre les rendez-vous.

Veuillez choisir une autre heure.
```

### 3. **Vérification pour le Patient**
Le système vérifie que le patient n'a pas déjà de rendez-vous :
- À l'heure exacte demandée
- Dans l'intervalle de ±10 minutes

**Message d'erreur :**
```
[Patient] a déjà un rendez-vous avec le Dr. [Nom] à [Heure]

Il doit y avoir au moins 10 minutes d'intervalle entre les rendez-vous.

Veuillez choisir une autre heure.
```

## Implémentation Technique

### Méthode `CheckAppointmentConflictAsync`

```csharp
public async Task<AppointmentConflict> CheckAppointmentConflictAsync(
    DateTime dateTime, 
    int patientId, 
    int medecinId, 
    int? currentAppointmentId = null)
```

**Paramètres :**
- `dateTime` : Date et heure du rendez-vous à vérifier
- `patientId` : ID du patient
- `medecinId` : ID du médecin
- `currentAppointmentId` : ID du rendez-vous en cours de modification (null pour une création)

**Retour :**
- `AppointmentConflict` : Objet contenant les informations sur le conflit détecté

### Classes de Support

#### `AppointmentConflict`
```csharp
public class AppointmentConflict
{
    public bool HasConflict { get; set; }
    public ConflictType ConflictType { get; set; }
    public Appointment ConflictingAppointment { get; set; }
    public string Message { get; set; }
}
```

#### `ConflictType`
```csharp
public enum ConflictType
{
    None,      // Pas de conflit
    Medecin,   // Conflit avec un autre RDV du médecin
    Patient    // Conflit avec un autre RDV du patient
}
```

## Exemples de Scénarios

### ✅ Scénario Valide
- RDV 1 : Patient A avec Dr. X à 10:00
- RDV 2 : Patient B avec Dr. X à 10:15
- **Résultat :** ✅ Autorisé (15 minutes d'écart)

### ❌ Scénario Invalide - Médecin Occupé
- RDV 1 : Patient A avec Dr. X à 10:00
- RDV 2 : Patient B avec Dr. X à 10:05
- **Résultat :** ❌ Refusé (seulement 5 minutes d'écart)

### ❌ Scénario Invalide - Patient Occupé
- RDV 1 : Patient A avec Dr. X à 10:00
- RDV 2 : Patient A avec Dr. Y à 10:08
- **Résultat :** ❌ Refusé (le patient ne peut pas être à deux endroits)

### ✅ Scénario Valide - Modification
- RDV existant : Patient A avec Dr. X à 10:00
- Modification : Changer l'heure à 10:05
- **Résultat :** ✅ Autorisé (le RDV en cours est exclu de la vérification)

## Intégration dans le Formulaire

Le contrôle est effectué dans `AppointmentFormViewModel.SaveAsync()` :

1. **Avant la sauvegarde** : Vérification des conflits
2. **Si conflit détecté** : Affichage d'une alerte bloquante (pas de possibilité de forcer)
3. **Si pas de conflit** : Sauvegarde du rendez-vous

## Modifications des Rendez-vous

Lors de la modification d'un rendez-vous existant :
- Le rendez-vous en cours de modification est **exclu** de la vérification
- Cela permet de changer d'autres attributs (type, statut, notes) sans conflit
- Mais empêche toujours les chevauchements avec d'autres rendez-vous

## Améliorations Futures Possibles

1. **Durée des Rendez-vous**
   - Actuellement : Intervalle fixe de 10 minutes
   - Amélioration : Durée configurable par type de rendez-vous

2. **Heures d'Ouverture**
   - Ajouter des contraintes sur les horaires de travail des médecins

3. **Jours Fériés et Congés**
   - Bloquer la création de rendez-vous pendant les périodes de fermeture

4. **Capacité de la Salle d'Attente**
   - Limiter le nombre de rendez-vous simultanés dans le cabinet

5. **Notifications**
   - Suggérer des créneaux horaires disponibles proches

## Notes Techniques

- Les vérifications sont effectuées au niveau de la **base de données** pour garantir l'intégrité
- Les comparaisons de dates utilisent des **intervalles inclusifs** (≥ et ≤)
- Les messages sont **localisés en français** avec des émojis pour une meilleure UX
- La méthode `HasCollisionAsync` originale est **conservée** pour compatibilité ascendante
