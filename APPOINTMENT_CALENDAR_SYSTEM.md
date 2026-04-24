# 📅 Système d'Agenda Visuel pour les Rendez-vous

## Vue d'ensemble

Le nouveau système d'agenda offre une interface visuelle intuitive pour la sélection des créneaux horaires lors de la création ou modification de rendez-vous.

## Fonctionnalités Principales

### 1. **Affichage Visuel des Créneaux**
- Grille de créneaux horaires de **8h00 à 18h00**
- Intervalles de **15 minutes** entre chaque créneau
- Affichage sur 4 colonnes pour une meilleure lisibilité

### 2. **Codes Couleur Intuitifs**

| Statut | Couleur | Icône | Description |
|--------|---------|-------|-------------|
| **Disponible** | 🟢 Vert (`#10B981`) | ✅ | Créneau libre, peut être sélectionné |
| **Occupé** | 🔴 Rouge (`#EF4444`) | ❌ | Médecin déjà en rendez-vous |
| **Passé** | ⚫ Gris (`#475569`) | ⏰ | Dans le passé, ne peut être sélectionné |

### 3. **Protection Automatique**

#### Empêcher les RDV dans le Passé
- `DatePicker` configuré avec `MinimumDate="{x:Static sys:DateTime.Today}"`
- Validation côté ViewModel si la date/heure est antérieure à `DateTime.Now`
- Message d'erreur explicite : *"Vous ne pouvez pas créer un rendez-vous dans le passé"*

#### Griser les Horaires Occupés
- Vérification en temps réel de la disponibilité du médecin
- Prise en compte de l'intervalle de 10 minutes
- Désactivation automatique des créneaux non disponibles

### 4. **Mise à Jour Dynamique**

Les créneaux se rechargent automatiquement lorsque :
- Le **médecin** sélectionné change
- La **date** sélectionnée change
- Le formulaire est ouvert pour modification

## Architecture Technique

### Nouvelle Classe : `TimeSlot`

```csharp
public class TimeSlot
{
    public TimeSpan Time { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsPast { get; set; }
    public string DisplayTime { get; } // Format "HH:mm"
    public string StatusIcon { get; } // ✅, ❌ ou ⏰
    public Color BackgroundColor { get; } // Couleur selon le statut
    public Color TextColor { get; } // Blanc pour tous
    public bool IsEnabled { get; } // false si passé ou occupé
}
```

### Méthode `LoadAvailableTimeSlotsAsync()`

Cette méthode :
1. Génère les créneaux de 8h à 18h par intervalles de 15 minutes
2. Récupère les rendez-vous existants du médecin pour la date sélectionnée
3. Marque comme occupés les créneaux dans l'intervalle de ±10 minutes
4. Marque comme passés les créneaux antérieurs à l'heure actuelle
5. Exclut le rendez-vous en cours de modification (si applicable)
6. Met à jour la collection `AvailableTimeSlots` sur le thread UI

### Interface XAML

```xaml
<CollectionView 
    ItemsSource="{Binding AvailableTimeSlots}"
    SelectionMode="Single"
    SelectedItem="{Binding SelectedTimeSlot}">

    <CollectionView.ItemsLayout>
        <GridItemsLayout 
            Orientation="Vertical" 
            Span="4"
            HorizontalItemSpacing="10"
            VerticalItemSpacing="10" />
    </CollectionView.ItemsLayout>

    <!-- Chaque créneau est affiché dans un Frame coloré -->
</CollectionView>
```

## Validation Améliorée

### Nouvelles Vérifications dans `SaveAsync()`

1. **Sélection d'un Créneau Obligatoire**
   ```csharp
   if (SelectedTimeSlot == null || !SelectedTimeSlot.IsEnabled)
   {
       await Shell.Current.DisplayAlert("Erreur", 
           "Veuillez sélectionner un créneau horaire disponible", "OK");
       return;
   }
   ```

2. **Vérification Anti-Passé**
   ```csharp
   if (dateTime < DateTime.Now)
   {
       await Shell.Current.DisplayAlert("⚠️ Date invalide", 
           "Vous ne pouvez pas créer un rendez-vous dans le passé.", "OK");
       return;
   }
   ```

3. **Vérification des Conflits**
   - La vérification existante avec `CheckAppointmentConflictAsync` reste active
   - Double sécurité : UI + Backend

## Expérience Utilisateur

### Workflow de Création d'un Rendez-vous

1. **Sélection du Patient** (obligatoire)
2. **Sélection du Médecin** (obligatoire)
   - → Chargement automatique des créneaux disponibles
3. **Sélection de la Date**
   - Date minimale = Aujourd'hui
   - → Rechargement des créneaux disponibles
4. **Sélection du Créneau Horaire**
   - Visualisation immédiate de la disponibilité
   - Clic sur un créneau vert pour le sélectionner
   - Les créneaux rouges et gris ne sont pas cliquables
5. **Sélection du Type et Statut** (optionnel avec valeurs par défaut)
6. **Ajout de Notes** (optionnel)
7. **Enregistrement**

### Messages d'Aide

Si aucun médecin ou date n'est sélectionné :
```
⚠️
Veuillez sélectionner un médecin et une date
```

Légende des couleurs affichée en permanence :
```
✅ Disponible  ❌ Occupé  ⏰ Passé
```

## Cas d'Usage Avancés

### Modification d'un Rendez-vous Existant
- Le créneau actuel est exclu de la vérification de disponibilité
- Permet de modifier d'autres attributs sans conflit
- Exemple : Changer le type de "Consultation" à "Urgence" à la même heure

### Plusieurs Rendez-vous le Même Jour
- L'agenda se met à jour pour chaque sélection
- Affiche uniquement les créneaux réellement disponibles
- Respect de l'intervalle de 10 minutes

### Navigation Rapide
- Changement de médecin : les créneaux se mettent à jour instantanément
- Changement de date : nouvelle grille de disponibilités
- Retour en arrière : état préservé

## Paramètres Configurables

Les constantes suivantes peuvent être modifiées dans `LoadAvailableTimeSlotsAsync()` :

```csharp
var startHour = 8;              // Heure de début (8h00)
var endHour = 18;               // Heure de fin (18h00)
var intervalMinutes = 15;       // Intervalle entre créneaux (15 min)
```

## Améliorations Futures Possibles

1. **Plages Horaires Personnalisées**
   - Heures d'ouverture différentes par médecin
   - Gestion des pauses déjeuner

2. **Durée Variable**
   - Créneaux de 30 minutes pour les consultations
   - Créneaux de 15 minutes pour les contrôles
   - Créneaux de 1 heure pour les urgences

3. **Vue Calendrier**
   - Affichage mensuel avec disponibilité globale
   - Navigation rapide entre les dates

4. **Statistiques**
   - Taux de remplissage par médecin
   - Créneaux les plus demandés

5. **Notifications**
   - Suggestion automatique du prochain créneau disponible
   - Alertes pour les créneaux bientôt disponibles

## Notes Techniques

- **Performance** : La méthode charge tous les rendez-vous du jour, ce qui est optimal pour la plupart des cabinets médicaux (< 100 RDV/jour)
- **Thread Safety** : Utilisation de `MainThread.BeginInvokeOnMainThread()` pour les mises à jour UI
- **Réactivité** : Les changements de sélection déclenchent automatiquement le rechargement
- **Accessibilité** : Les créneaux désactivés ne répondent pas aux interactions

## Compatibilité

- ✅ .NET 9
- ✅ .NET MAUI
- ✅ Windows, iOS, Android, macOS
- ✅ SQLite-net pour la persistance
