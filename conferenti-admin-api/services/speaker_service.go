package services

import (
	"context"
	"time"

	"github.com/google/uuid"
	"github.com/kkho/conferenti/conferenti-admin-api/models"
	"github.com/kkho/conferenti/conferenti-admin-api/repositories"
)

type SpeakerService struct {
	repo *repositories.SpeakerRepository
}

func NewSpeakerService(repo *repositories.SpeakerRepository) *SpeakerService {
	service := &SpeakerService{
		repo: repo,
	}

	return service
}

func (speakerService *SpeakerService) Create(ctx context.Context, speaker *models.Speaker) *models.Speaker {
	if speaker.Id == "" {
		speaker.Id = uuid.New().String()
	}
	speaker.CreatedAt = time.Now()
	speaker.UpdatedAt = time.Now()

	err := speakerService.repo.Create(ctx, speaker)
	if err != nil {
		// Log error but return the speaker anyway for debugging
		// In production, you might want to return error
		return speaker
	}

	return speaker
}

func (speakerService *SpeakerService) GetAll(ctx context.Context) ([]*models.Speaker, error) {
	return speakerService.repo.GetAll(ctx)
}

func (speakerService *SpeakerService) Update(ctx context.Context, speaker *models.Speaker) (*models.Speaker, error) {
	return speakerService.repo.Update(ctx, speaker)
}

func (speakerService *SpeakerService) Delete(ctx context.Context, id string) (string, error) {
	return speakerService.repo.Delete(ctx, id)
}
