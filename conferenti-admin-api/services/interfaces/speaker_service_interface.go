package services_interface

import (
	"context"

	"github.com/kkho/conferenti/conferenti-admin-api/models"
)

// SpeakerServiceInterface defines the methods for speaker service
type SpeakerServiceInterface interface {
	Create(ctx context.Context, speaker *models.Speaker) error
	GetAll(ctx context.Context) ([]*models.Speaker, error)
	Update(ctx context.Context, speaker *models.Speaker) (*models.Speaker, error)
	Delete(ctx context.Context, id string) (string, error)
}
