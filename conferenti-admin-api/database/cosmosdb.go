package database

import (
	"context"
	"fmt"
	"log"

	"github.com/Azure/azure-sdk-for-go/sdk/azidentity"
	"github.com/Azure/azure-sdk-for-go/sdk/data/azcosmos"
	"github.com/kkho/conferenti/conferenti-admin-api/config"
)

type CosmosClient struct {
	Client           *azcosmos.Client
	Database         *azcosmos.DatabaseClient
	SpeakerContainer *azcosmos.ContainerClient
	SessionContainer *azcosmos.ContainerClient
}

func NewCosmosClient(ctx context.Context, cfg config.Config) (*CosmosClient, error) {
	var client *azcosmos.Client
	var err error

	clientOptions := azcosmos.ClientOptions{
		EnableContentResponseOnWrite: true,
	}

	if cfg.IsLocal {
		keyCredential, err := azcosmos.NewKeyCredential(cfg.CosmosDb.Key)

		if err != nil {
			log.Fatalf("failed to create key credential: %v", err)
		}

		client, err = azcosmos.NewClientWithKey(cfg.CosmosDb.Endpoint, keyCredential, &clientOptions)
		if err != nil {
			log.Fatalf("failed to create Cosmos DB client: %v", err)
		}

	} else {
		cred, err := azidentity.NewDefaultAzureCredential(nil)
		if err != nil {
			log.Fatalf("failed to create credential: %v", err)
		}

		client, err = azcosmos.NewClient(cfg.CosmosDb.Endpoint, cred, &clientOptions)
		if err != nil {
			log.Fatalf("failed to create Cosmos DB client: %v", err)
		}
	}

	database, err := client.NewDatabase(cfg.CosmosDb.Database)
	if err != nil {
		fmt.Printf("Error creating database %s: %v\n", cfg.CosmosDb.Database, err)
	} else {
		fmt.Printf("Successfully created database: %s\n", database.ID())
	}

	speakercontainer, _err := client.NewContainer(database.ID(), cfg.CosmosDb.SpeakerContainer)
	if _err != nil {
		log.Fatalf("Failed to create container client: %v", _err)
	}

	sessioncontainer, _err := client.NewContainer(database.ID(), cfg.CosmosDb.SessionContainer)
	if _err != nil {
		log.Fatalf("Failed to create container client: %v", _err)
	}

	return &CosmosClient{
		Client:           client,
		Database:         database,
		SpeakerContainer: speakercontainer,
		SessionContainer: sessioncontainer,
	}, nil

}

func (c *CosmosClient) Close() error {
	return nil
}
