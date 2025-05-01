# TaskFlow API

## Description
TaskFlow est une API REST développée en ASP.NET Core pour la gestion de projets et de tâches. Elle permet aux utilisateurs de créer des projets, d'ajouter des tâches et de suivre leur progression.

## Technologies Utilisées
- ASP.NET Core
- Entity Framework Core avec SQLite
- JWT pour l'authentification
- BCrypt pour le hachage des mots de passe
- Swagger pour la documentation API

## Structure du Projet

### Modèles de Données
1. **User**
   - Id (int, clé primaire)
   - Name (string, obligatoire)
   - Email (string, obligatoire, unique)
   - PasswordHash (string, obligatoire)
   - Role (enum: User/Admin)
   - Projects (Collection)

2. **Project**
   - Id (int, clé primaire)
   - Name (string, obligatoire)
   - Description (string, optionnel)
   - CreationDate (DateTime, auto-généré)
   - UserId (int, clé étrangère)
   - Tasks (Collection)

3. **Task**
   - Id (int, clé primaire)
   - Title (string, obligatoire)
   - Description (string)
   - Status (string: ToDo, InProgress, Done)
   - DueDate (DateTime)
   - ProjectId (int, clé étrangère)
   - Comments (Collection de strings)

## Authentification et Autorisation

### Endpoints Publics
Seuls deux endpoints sont accessibles sans authentification :

1. **Inscription** : `POST /api/users/register`
```json
{
  "name": "user",
  "email": "user@example.com",
  "passwordHash": "password123"
}
```

2. **Connexion** : `POST /api/users/login?username=user&password=password123`

### Utilisation du Token JWT
Pour toutes les autres routes, le token JWT doit être inclus dans le header Authorization avec le préfixe "Bearer" :
```
Authorization: Bearer votre_token_jwt
```

## Points d'API Sécurisés

### Projets
Tous ces endpoints nécessitent une authentification JWT.

- `GET /api/projects` : Liste tous les projets de l'utilisateur
- `GET /api/projects/{id}` : Récupère un projet spécifique
- `POST /api/projects` : Crée un nouveau projet
```json
{
  "name": "Mon Projet",
  "description": "Description du projet"
}
```
- `PUT /api/projects/{id}` : Met à jour un projet existant
- `DELETE /api/projects/{id}` : Supprime un projet

### Tâches
Tous ces endpoints nécessitent une authentification JWT.

- `GET /api/tasks` : Liste toutes les tâches
- `GET /api/tasks/{id}` : Récupère une tâche spécifique
- `POST /api/tasks` : Crée une nouvelle tâche
```json
{
  "title": "Ma Tâche",
  "description": "Description de la tâche",
  "status": "ToDo",
  "projectId": 1,
  "dueDate": "2024-12-31T23:59:59Z"
}
```
- `PUT /api/tasks/{id}` : Met à jour une tâche existante
- `DELETE /api/tasks/{id}` : Supprime une tâche
- `POST /api/tasks/{id}/comments` : Ajoute un commentaire

## Sécurité Actuelle

1. **Authentification**
   - Basée sur JWT avec une durée de validité de 24h
   - Hachage des mots de passe avec BCrypt
   - Validation des tokens sur chaque requête

2. **Autorisations**
   - Système de rôles (User/Admin)
   - Protection de toutes les routes avec [Authorize]
   - Vérification du propriétaire pour les projets

## Améliorations Possibles

1. **Sécurité**
   - Ajouter un système de refresh tokens
   - Mettre en place une liste noire de tokens révoqués
   - Renforcer la politique de mots de passe
   - Ajouter la validation des adresses email
   - Implémenter la limitation de taux (rate limiting)

2. **Autorisations**
   - Ajouter des permissions par projet (lecture/écriture)
   - Permettre le partage de projets entre utilisateurs
   - Créer des rôles personnalisés
   - Ajouter des quotas (nombre max de projets/tâches)

## Installation et Configuration

1. Cloner le repository
2. Restaurer les packages :
```bash
dotnet restore
```
3. Configurer la base de données dans appsettings.json
4. Lancer l'application :
```bash
dotnet run
```
5. Accéder à Swagger UI : `https://localhost:5001/swagger`

## Notes Importantes

- Les tokens JWT doivent toujours être préfixés par "Bearer " dans le header
- Toutes les dates sont en UTC
- La base de données SQLite est créée automatiquement au démarrage
- Les mots de passe sont hachés avant stockage
- Les ID sont auto-générés, ne pas les inclure dans les requêtes POST