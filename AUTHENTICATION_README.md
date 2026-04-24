# 🔐 Système d'Authentification - Guide

## 📋 Vue d'ensemble

Le système d'authentification a été implémenté avec les fonctionnalités suivantes :
- ✅ Connexion avec email et mot de passe
- ✅ Inscription de nouveaux utilisateurs
- ✅ Hashage sécurisé des mots de passe (SHA256)
- ✅ Session persistante (l'utilisateur reste connecté)
- ✅ Déconnexion
- ✅ Protection de l'accès à l'application

## 🚀 Première Utilisation

### Créer le premier compte

1. **Lancez l'application**
2. Vous verrez la **page de connexion** avec :
   - Logo du cabinet médical (📅)
   - Formulaire de connexion
   - Bouton "Créer un compte"

3. **Pour créer votre premier compte :**
   - Cliquez sur **"Créer un compte"**
   - Entrez votre **email** (ex: admin@cabinet.fr)
   - Entrez un **mot de passe** (minimum 6 caractères)
   - Cliquez sur **"Créer un compte"**
   - Vous serez automatiquement connecté et redirigé vers le Dashboard

## 🔒 Sécurité

### Hashage des mots de passe
Les mots de passe sont hashés avec **SHA256** avant d'être stockés dans la base de données. Le mot de passe en clair n'est jamais sauvegardé.

### Session persistante
Lorsque vous vous connectez, votre session est sauvegardée dans les **Preferences** de MAUI. Vous restez connecté même si vous fermez l'application.

## 📱 Fonctionnalités

### Page de Connexion
- **Email** : Champ avec icône 📧
- **Mot de passe** : Champ masqué avec icône 🔒
- **Messages d'erreur** : Affichage en rouge si email/mot de passe incorrect
- **Indicateur de chargement** : ActivityIndicator pendant l'authentification
- **Création de compte** : Bouton secondaire pour s'inscrire

### Dashboard
- **Bouton "📂 DB Info"** : Affiche le chemin de la base de données
- **Bouton "🚪 Déconnexion"** : Déconnecte l'utilisateur et retourne à la page de connexion

## 🗄️ Structure de la Base de Données

### Table `User`
| Colonne | Type | Description |
|---------|------|-------------|
| `Id` | INTEGER | Clé primaire auto-incrémentée |
| `Email` | TEXT | Email unique (identifiant de connexion) |
| `PasswordHash` | TEXT | Hash SHA256 du mot de passe |
| `FullName` | TEXT | Nom complet de l'utilisateur |
| `CreatedAt` | DATETIME | Date de création du compte |
| `LastLoginAt` | DATETIME | Dernière connexion |

## 🔧 Services Implémentés

### AuthenticationService
```csharp
- HashPassword(string password) : string
- VerifyPassword(string password, string hash) : bool
- SaveCurrentUser(int userId) : void
- GetCurrentUserId() : int?
- IsUserLoggedIn() : bool
- Logout() : void
```

### DatabaseService (méthodes ajoutées)
```csharp
- GetUserByEmailAsync(string email) : Task<User>
- GetUserByIdAsync(int id) : Task<User>
- AddUserAsync(User user) : Task<int>
- UpdateUserAsync(User user) : Task
- GetUserCountAsync() : Task<int>
```

## 🎨 Design

La page de connexion utilise le même design que le reste de l'application :
- **Background sombre** : #0F172A
- **Carte de formulaire** : #1E293B
- **Bouton principal** : #2563EB (bleu)
- **Bouton secondaire** : #475569 (gris)
- **Logo circulaire** avec émoji 📅

## 💡 Conseils

### Pour tester
- **Email de test** : admin@test.fr
- **Mot de passe de test** : admin123

### Pour réinitialiser
Si vous voulez supprimer tous les utilisateurs et recommencer :
1. Cliquez sur "📂 DB Info" pour obtenir le chemin
2. Fermez l'application
3. Supprimez le fichier `gestion_rdv.db`
4. Relancez l'application

## ✨ Améliorations Futures Possibles

- 📧 Récupération de mot de passe par email
- 👤 Gestion de profil utilisateur
- 🔐 Authentification à deux facteurs (2FA)
- 👥 Rôles et permissions (Admin, Médecin, Secrétaire)
- 📱 Authentification biométrique (empreinte, Face ID)
- 🔑 Changement de mot de passe
- ⏱️ Expiration de session après inactivité
