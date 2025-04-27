# Codes HTTP utilisés dans l'API TaskFlow

## 2xx Success

### 200 OK
- Utilisé pour les requêtes GET réussies
- Retourné lors d'une authentification réussie
- Exemple : GET /api/projects

### 201 Created
- Utilisé lors de la création réussie d'une ressource
- Retourne l'URL de la nouvelle ressource dans l'en-tête Location
- Exemple : POST /api/projects

### 204 No Content
- Utilisé pour les opérations de mise à jour et suppression réussies
- Exemple : DELETE /api/tasks/{id}

## 4xx Client Errors

### 400 Bad Request
- Données de requête invalides
- Validation échouée
- Exemple : Tentative d'enregistrement avec un email déjà existant

### 401 Unauthorized
- Token JWT manquant ou invalide
- Tentative de connexion avec des identifiants invalides

### 403 Forbidden
- L'utilisateur est authentifié mais n'a pas les droits nécessaires
- Exemple : Tentative de modification d'un projet d'un autre utilisateur

### 404 Not Found
- Ressource non trouvée (projet, tâche, etc.)
- Exemple : GET /api/projects/{id} avec un ID inexistant

## 5xx Server Errors

### 500 Internal Server Error
- Erreur non gérée sur le serveur
- Problème de connexion à la base de données
- Exception inattendue

## Justification des choix

1. **Utilisation de 201 pour les créations**
   - Conforme aux standards REST
   - Permet au client de récupérer l'URL de la nouvelle ressource

2. **204 pour les suppressions et mises à jour**
   - Pas besoin de retourner de contenu
   - Économie de bande passante

3. **401 vs 403**
   - 401 : L'utilisateur n'est pas authentifié
   - 403 : L'utilisateur est authentifié mais n'a pas les droits

4. **500 avec message générique**
   - Ne pas exposer les détails techniques des erreurs
   - Les erreurs sont loggées pour le débogage
