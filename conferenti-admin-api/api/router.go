package api

import (
	"net/http"

	"github.com/Azure/azure-sdk-for-go/sdk/data/azcosmos"
	"github.com/gorilla/mux"
	"github.com/kkho/conferenti/conferenti-admin-api/config"
	"github.com/kkho/conferenti/conferenti-admin-api/handlers"
	"github.com/kkho/conferenti/conferenti-admin-api/middleware"
	"github.com/kkho/conferenti/conferenti-admin-api/repositories"
	"github.com/kkho/conferenti/conferenti-admin-api/services"
)

type Dependencies struct {
	SessionContainer *azcosmos.ContainerClient
	SpeakerContainer *azcosmos.ContainerClient

	SessionRepository *repositories.SessionRepository
	SpeakerRepository *repositories.SpeakerRepository

	SessionService *services.SessionService
	SpeakerService *services.SpeakerService

	HealthHandler  *handlers.HealthHandler
	SessionHandler *handlers.SessionHandler
	SpeakerHandler *handlers.SpeakerHandler
}

func NewDependencies(sessionContainer *azcosmos.ContainerClient, speakerContainer *azcosmos.ContainerClient) *Dependencies {
	deps := &Dependencies{
		SessionContainer: sessionContainer,
		SpeakerContainer: speakerContainer,
	}

	deps.SessionRepository = repositories.NewSessionRepository(deps.SessionContainer)
	deps.SpeakerRepository = repositories.NewSpeakerRepository(deps.SpeakerContainer)

	deps.SessionService = services.NewSessionService(deps.SessionRepository)
	deps.SpeakerService = services.NewSpeakerService(deps.SpeakerRepository)
	deps.HealthHandler = handlers.NewHealthHandler()
	deps.SessionHandler = handlers.NewSessionHandler(deps.SessionService)
	deps.SpeakerHandler = handlers.NewSpeakerHandler(deps.SpeakerService)

	return deps
}

func NewRouter(deps *Dependencies, cfg *config.Config) *mux.Router {
	router := mux.NewRouter()
	authMiddleware := middleware.NewAuth0Middleware(cfg.Auth0.Domain, cfg.Auth0.Audience)

	api := router.PathPrefix("/api/v1").Subrouter()
	// Apply middlewares
	router.Use(middleware.Logging)
	router.Use(middleware.CORS)
	router.Use(middleware.ContentType)
	api.HandleFunc("/health", deps.HealthHandler.Check).Methods("GET")

	// Session routes
	sessionRoutes := api.PathPrefix("/sessions").Subrouter()
	sessionRoutes.Use(authMiddleware.CheckJWT) // All session routes need auth
	sessionRoutes.Handle("",
		middleware.RequireScope("admin:execute")(
			http.HandlerFunc(deps.SessionHandler.CreateSession))).Methods("POST")
	sessionRoutes.Handle("",
		middleware.RequireScope("admin:execute")(
			http.HandlerFunc(deps.SessionHandler.GetSessions))).Methods("GET")
	sessionRoutes.Handle("/{id}",
		middleware.RequireScope("admin:execute")(
			http.HandlerFunc(deps.SessionHandler.GetSessionById))).Methods("GET")
	sessionRoutes.Handle("/{id}",
		middleware.RequireScope("admin:execute")(
			http.HandlerFunc(deps.SessionHandler.DeleteSession))).Methods("DELETE")

	// Speaker routes
	speakerRoutes := api.PathPrefix("/speakers").Subrouter()
	speakerRoutes.Use(authMiddleware.CheckJWT) // All speaker routes need auth
	speakerRoutes.Handle("",
		middleware.RequireScope("admin:execute")(
			http.HandlerFunc(deps.SpeakerHandler.CreateSpeaker))).Methods("POST")
	speakerRoutes.Handle("",
		middleware.RequireScope("admin:execute")(
			http.HandlerFunc(deps.SpeakerHandler.GetSpeakers))).Methods("GET")
	speakerRoutes.Handle("/{id}",
		middleware.RequireScope("admin:execute")(
			http.HandlerFunc(deps.SpeakerHandler.GetSpeakerById))).Methods("GET")
	speakerRoutes.Handle("/{id}",
		middleware.RequireScope("admin:execute")(
			http.HandlerFunc(deps.SpeakerHandler.DeleteSpeaker))).Methods("DELETE")

	return router
}
