package services

import (
	"context"
	"time"

	"github.com/google/uuid"
	"github.com/kkho/conferenti/conferenti-admin-api/models"
	"github.com/kkho/conferenti/conferenti-admin-api/repositories"
)

type SessionService struct {
	repo *repositories.SessionRepository
}

func NewSessionService(sessionRepository *repositories.SessionRepository) *SessionService {
	service := &SessionService{
		repo: sessionRepository,
	}

	return service
}

func (sessionService *SessionService) Create(ctx context.Context, session *models.Session) error {
	if session.SessionId == "" {
		session.SessionId = uuid.New().String()
	}
	session.SessionId = uuid.New().String()
	session.CreatedAt = time.Now()
	session.UpdatedAt = time.Now()
	return sessionService.repo.Create(ctx, session)
}

func (sessionService *SessionService) GetAll(ctx context.Context) ([]*models.Session, error) {
	return sessionService.repo.GetAll(ctx)
}

func (sessionService *SessionService) Update(ctx context.Context, session *models.Session) (*models.Session, error) {
	return sessionService.repo.Update(ctx, session)
}

func (sessionService *SessionService) Delete(ctx context.Context, id string) (string, error) {
	return sessionService.repo.Delete(ctx, id)
}
