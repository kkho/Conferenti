# Conference API Project Structure

This document describes a recommended project structure for a modern Node.js REST API for a conference management system ("conference-api").

## Root Structure

```
conference-api/
├── README.md
├── package.json
├── tsconfig.json
├── .env
├── .gitignore
├── src/
│   ├── index.ts
│   ├── app.ts
│   ├── server.ts
│   ├── config/
│   │   └── index.ts
│   ├── routes/
│   │   ├── speakers.ts
│   │   ├── sessions.ts
│   │   ├── auth.ts
│   │   └── ...
│   ├── controllers/
│   │   ├── speakerController.ts
│   │   ├── sessionController.ts
│   │   └── ...
│   ├── models/
│   │   ├── speaker.ts
│   │   ├── session.ts
│   │   └── ...
│   ├── middlewares/
│   │   ├── authMiddleware.ts
│   │   ├── errorHandler.ts
│   │   └── ...
│   ├── services/
│   │   ├── speakerService.ts
│   │   ├── sessionService.ts
│   │   └── ...
│   ├── utils/
│   │   └── ...
│   └── types/
│       └── index.d.ts
├── tests/
│   ├── integration/
│   │   └── ...
│   └── unit/
│       └── ...
└── scripts/
    └── seed.ts
```

## Folder Descriptions

- **src/**: Main source code for the API
  - **index.ts**: Entry point
  - **app.ts**: Express app setup
  - **server.ts**: Server bootstrap
  - **config/**: Configuration (env, DB, etc.)
  - **routes/**: Express route definitions
  - **controllers/**: Request handlers
  - **models/**: Data models (e.g., Mongoose, Prisma, Sequelize)
  - **middlewares/**: Express middlewares
  - **services/**: Business logic
  - **utils/**: Utility/helper functions
  - **types/**: TypeScript type definitions
- **tests/**: Unit and integration tests
- **scripts/**: Utility scripts (e.g., DB seeding)

## Example Endpoints

- `GET /speakers` — List all speakers
- `GET /sessions` — List all sessions
- `POST /speakers` — Add a new speaker
- `POST /sessions` — Add a new session
- `GET /speakers/:id` — Get speaker details
- `GET /sessions/:id` — Get session details

---

This structure is scalable, testable, and ready for modern TypeScript Node.js development.
