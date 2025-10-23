package config

import (
	"os"
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestLoad_DefaultValues(t *testing.T) {
	// Clear environment variables
	os.Clearenv()

	// Act
	cfg := Load()

	// Assert
	assert.NotNil(t, cfg)
	assert.Equal(t, "8084", cfg.Port)
	assert.Equal(t, "development", cfg.Environment)
	assert.True(t, cfg.IsLocal)
	assert.NotNil(t, cfg.Auth0)
	assert.NotNil(t, cfg.CosmosDb)
}

func TestLoad_WithEnvironmentVariables(t *testing.T) {
	// Arrange
	os.Setenv("PORT", "9090")
	os.Setenv("ENVIRONMENT", "production")
	os.Setenv("LOCAL", "false")
	os.Setenv("AUTH0_DOMAIN", "test.auth0.com")
	os.Setenv("AUTH0_AUDIENCE", "https://test.api.com")
	os.Setenv("COSMOSDB_ENDPOINT", "https://test.cosmos.azure.com")
	os.Setenv("COSMOSDB_DATABASE", "TestDatabase")

	defer func() {
		// Cleanup
		os.Clearenv()
	}()

	// Act
	cfg := Load()

	// Assert
	assert.Equal(t, "9090", cfg.Port)
	assert.Equal(t, "production", cfg.Environment)
	assert.False(t, cfg.IsLocal)
	assert.Equal(t, "test.auth0.com", cfg.Auth0.Domain)
	assert.Equal(t, "https://test.api.com", cfg.Auth0.Audience)
	assert.Equal(t, "https://test.cosmos.azure.com", cfg.CosmosDb.Endpoint)
	assert.Equal(t, "TestDatabase", cfg.CosmosDb.Database)
}

func TestGetEnv(t *testing.T) {
	tests := []struct {
		name         string
		key          string
		defaultValue string
		envValue     string
		expected     string
	}{
		{
			name:         "Environment variable exists",
			key:          "TEST_KEY",
			defaultValue: "default",
			envValue:     "environment_value",
			expected:     "environment_value",
		},
		{
			name:         "Environment variable does not exist",
			key:          "NON_EXISTENT_KEY",
			defaultValue: "default_value",
			envValue:     "",
			expected:     "default_value",
		},
		{
			name:         "Environment variable is empty",
			key:          "EMPTY_KEY",
			defaultValue: "default",
			envValue:     "",
			expected:     "default",
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			// Arrange
			if tt.envValue != "" {
				os.Setenv(tt.key, tt.envValue)
				defer os.Unsetenv(tt.key)
			}

			// Act
			result := getEnv(tt.key, tt.defaultValue)

			// Assert
			assert.Equal(t, tt.expected, result)
		})
	}
}

func TestGetEnvAsBool(t *testing.T) {
	tests := []struct {
		name         string
		key          string
		defaultValue bool
		envValue     string
		expected     bool
	}{
		{
			name:         "True value 'true'",
			key:          "BOOL_KEY",
			defaultValue: false,
			envValue:     "true",
			expected:     true,
		},
		{
			name:         "True value '1'",
			key:          "BOOL_KEY",
			defaultValue: false,
			envValue:     "1",
			expected:     true,
		},
		{
			name:         "False value 'false'",
			key:          "BOOL_KEY",
			defaultValue: true,
			envValue:     "false",
			expected:     false,
		},
		{
			name:         "False value '0'",
			key:          "BOOL_KEY",
			defaultValue: true,
			envValue:     "0",
			expected:     false,
		},
		{
			name:         "Invalid value defaults to false",
			key:          "BOOL_KEY",
			defaultValue: true,
			envValue:     "invalid",
			expected:     false,
		},
		{
			name:         "Empty value uses default",
			key:          "NON_EXISTENT_BOOL",
			defaultValue: true,
			envValue:     "",
			expected:     true,
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			// Arrange
			if tt.envValue != "" {
				os.Setenv(tt.key, tt.envValue)
				defer os.Unsetenv(tt.key)
			} else {
				os.Unsetenv(tt.key)
			}

			// Act
			result := getEnvAsBool(tt.key, tt.defaultValue)

			// Assert
			assert.Equal(t, tt.expected, result)
		})
	}
}

func TestConfig_AllFieldsPresent(t *testing.T) {
	// Act
	cfg := Load()

	// Assert - Verify all config sections are initialized
	assert.NotNil(t, cfg.Auth0)
	assert.NotNil(t, cfg.CosmosDb)

	// Auth0 config should have all fields (even if empty)
	assert.NotNil(t, cfg.Auth0.Secret)
	assert.NotNil(t, cfg.Auth0.Domain)
	assert.NotNil(t, cfg.Auth0.ClientId)
	assert.NotNil(t, cfg.Auth0.ClientSecret)
	assert.NotNil(t, cfg.Auth0.Audience)
	assert.NotNil(t, cfg.Auth0.Scope)

	// CosmosDB config should have all fields (even if empty)
	assert.NotNil(t, cfg.CosmosDb.Endpoint)
	assert.NotNil(t, cfg.CosmosDb.Key)
	assert.NotNil(t, cfg.CosmosDb.Database)
	assert.NotNil(t, cfg.CosmosDb.SpeakerContainer)
	assert.NotNil(t, cfg.CosmosDb.SessionContainer)
}
