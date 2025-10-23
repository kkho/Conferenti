package config

import (
	"log"
	"os"

	"github.com/joho/godotenv"
)

type Config struct {
	Port        string
	Environment string
	IsLocal     bool
	CosmosDb    *CosmosDbConfig
	Auth0       *Auth0Config
}

type Auth0Config struct {
	Secret       string
	Domain       string
	ClientId     string
	ClientSecret string
	Audience     string
	Scope        string
}

type CosmosDbConfig struct {
	Endpoint         string
	Key              string
	Database         string
	SpeakerContainer string
	SessionContainer string
}

func Load() *Config {
	// Try to load .env.local first, then .env from multiple locations
	locations := []string{
		".",     // Current directory
		"..",    // Parent directory
		"../..", // Two levels up (for when running from cmd/admin-api)
	}

	envFiles := []string{".env.local", ".env"}
	var loadedFile string

	for _, location := range locations {
		for _, envFile := range envFiles {
			var envPath string
			if location == "." {
				envPath = envFile
			} else {
				envPath = location + "/" + envFile
			}
			if err := godotenv.Load(envPath); err == nil {
				loadedFile = envPath
				log.Printf("Loaded environment variables from: %s", envPath)
				break
			}
		}
		if loadedFile != "" {
			break
		}
	}

	if loadedFile == "" {
		log.Println("No .env file found, using system environment variables")
	}

	return &Config{
		Port:        getEnv("PORT", "8084"),
		Environment: getEnv("ENVIRONMENT", "development"),
		IsLocal:     getEnvAsBool("LOCAL", true),

		Auth0: &Auth0Config{
			Secret:       getEnv("AUTH0_SECRET", ""),
			Domain:       getEnv("AUTH0_DOMAIN", ""),
			ClientId:     getEnv("AUTH0_CLIENT_ID", ""),
			ClientSecret: getEnv("AUTH0_CLIENT_SECRET", ""),
			Audience:     getEnv("AUTH0_AUDIENCE", ""),
			Scope:        getEnv("AUTH0_SCOPE", ""),
		},

		CosmosDb: &CosmosDbConfig{
			Endpoint:         getEnv("COSMOSDB_ENDPOINT", ""),
			Key:              getEnv("COSMOSDB_KEY", ""),
			Database:         getEnv("COSMOSDB_DATABASE", ""),
			SpeakerContainer: getEnv("COSMOSDB_SPEAKER_CONTAINER", ""),
			SessionContainer: getEnv("COSMOSDB_SESSION_CONTAINER", ""),
		},
	}
}

func getEnv(key, defaultValue string) string {
	if value := os.Getenv(key); value != "" {
		return value
	}
	return defaultValue
}

func getEnvAsBool(key string, defaultValue bool) bool {
	valStr := os.Getenv(key)
	if valStr == "" {
		return defaultValue
	}
	if valStr == "true" || valStr == "1" {
		return true
	}
	return false
}
