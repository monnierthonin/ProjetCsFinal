Contexte
L'entreprise TaskFlow est une startup spécialisée dans la gestion de tâches et de projets en ligne. Elle souhaite proposer une API REST permettant à des clients (applications web, mobiles ou autres) d'interagir avec son système de gestion de tâches.

TaskFlow a fait appel à vous en tant que prestataire pour développer la première version de son backend.

Votre mission est de concevoir et implémenter cette API en utilisant ce que vous avez appris pendant ce cours.

Vous pouvez constituez des groupes de deux personnes maximum.


Objectifs du Projet
Le projet consiste à développer une API REST complète permettant de gérer des projets et des tâches, tout en respectant les bonnes pratiques de développement en C#.


L'API devra proposer les fonctionnalités suivantes :

1. Modélisation des Entités
(Utilisation d'EntityFramework / SGBD SQL Server obligatoire)

Une entité User pour gérer l'authentification / autorisations :

Id (int, clé primaire)

Name (string, obligatoire)

Email (string, obligatoire, unique)

PasswordHash (string, obligatoire)

Role (enum : Admin, User)

Projects (collection des projets appartenant à l'utilisateur)

Une entité Project avec les propriétés suivantes :

Id (int, clé primaire)

Name (string, obligatoire)

Description (string, optionnel)

CreationDate (DateTime, auto-généré)

User (clé étrangère vers l'Utilisateur propriétaire)

Tasks (collection de tâches associées)

Une entité Task avec :

Id (int, clé primaire)

Title (string, obligatoire)

Status (enum : À faire, En cours, Terminé)

Project (clé étrangère vers Projet)

DueDate (DateTime, optionnel)

Commentaire (collection de strings pour suivre les notes)

Il vous sera demandé comment la base a été générée à l'aide d'EF. Il faudra donc soutenir votre solution.


2. API REST
Création de contrôleurs REST pour gérer les Users, Projects et Tasks.
Mise en place de l'authentification avec JWT pour restreindre l'accès à certains endpoints.

Implémentation des endpoints suivants pour les utilisateurs non authentifiés :

POST /api/users/register : Inscription d'un utilisateur.

POST /api/users/login : Connexion et obtention d'un token JWT.

Implémentation des endpoints suivants pour les utilisateurs authentifiés :

GET /api/projects : Liste tous les projets.

POST /api/projects : Ajoute un projet.

GET /api/projects/{id} : Récupère un projet par son ID.

PUT /api/projects/{id} : Met à jour un projet existant.

DELETE /api/projets/{id} : Supprime un projet (uniquement pour le propriétaire).

GET /api/tasks : Liste toutes les tâches.

POST /api/tasks : Ajoute une tâche à un projet.

GET /api/tasks/{id} : Récupère une tâche par son ID.

PUT /api/tasks/{id} : Met à jour une tâche existante.

DELETE /api/tasks/{id} : Supprime une tâche.


3. Gestion des Erreurs
Il vous faudra mettre en place un middleware pour la gestion des exceptions et utiliser sur tous les endpoints des codes retours HTTP que vous jugerez utiles (qu'il faudra justifier).


4. Documentation Swagger
Vous mettrez en place tout ce que vous avez pu apprendre dans le cours pour mettre en place la documentation Swagger.

La notation se fera en fonction de ce qui sera implémenté et ce qui manquera.