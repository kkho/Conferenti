package main

import (
	"context"
	"log"
	"net/http"

	"github.com/kkho/conferenti/conferenti-admin-api/api"
	"github.com/kkho/conferenti/conferenti-admin-api/config"
	"github.com/kkho/conferenti/conferenti-admin-api/database"
)

func main() {
	// Create context with cancellation - FIXED syntax
	ctx, cancel := context.WithCancel(context.Background())
	defer cancel()

	cfg := config.Load()

	cosmosClient, err := database.NewCosmosClient(
		ctx,
		*cfg)
	if err != nil {
		log.Fatalf("Failed to initialize Cosmos DB: %v", err)
	}
	defer cosmosClient.Close()

	log.Println("Conferenti Admin API starting...")
	deps := api.NewDependencies(cosmosClient.SessionContainer, cosmosClient.SpeakerContainer)
	router := api.NewRouter(deps, cfg)

	server := &http.Server{
		Addr:    ":" + cfg.Port,
		Handler: router,
	}

	if err := server.ListenAndServe(); err != nil {
		log.Fatal(err)
	}
}
