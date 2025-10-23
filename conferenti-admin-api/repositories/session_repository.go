package repositories

import (
	"context"
	"encoding/json"
	"fmt"

	"github.com/Azure/azure-sdk-for-go/sdk/data/azcosmos"
	"github.com/kkho/conferenti/conferenti-admin-api/models"
)

type SessionRepository struct {
	container *azcosmos.ContainerClient
}

func NewSessionRepository(container *azcosmos.ContainerClient) *SessionRepository {
	return &SessionRepository{
		container: container,
	}
}

func (repository *SessionRepository) Create(ctx context.Context, session *models.Session) error {
	sessionJSON, err := json.Marshal(session)

	if err != nil {
		return fmt.Errorf("failed to marshal session: %w", err)
	}

	partitionKey := azcosmos.NewPartitionKeyString(session.SessionId)

	_, err = repository.container.CreateItem(ctx, partitionKey, sessionJSON, nil)

	if err != nil {
		return fmt.Errorf("failed to create session: %w", err)
	}

	return nil
}

func (repository *SessionRepository) GetAll(ctx context.Context) ([]*models.Session, error) {
	query := "SELECT * FROM c"
	queryOptions := &azcosmos.QueryOptions{}

	queryPager := repository.container.NewQueryItemsPager(
		query,
		azcosmos.PartitionKey{},
		queryOptions)

	sessions := make([]*models.Session, 0)

	for queryPager.More() {
		queryResponse, err := queryPager.NextPage(ctx)
		if err != nil {
			return nil, fmt.Errorf("failed to query sessions: %w", err)
		}

		for _, item := range queryResponse.Items {
			var session models.Session
			if err := json.Unmarshal(item, &session); err != nil {
				return nil, fmt.Errorf("failed to unmarshal session: %w", err)
			}
			sessions = append(sessions, &session)
		}

	}

	return sessions, nil
}

func (repository *SessionRepository) Update(ctx context.Context, session *models.Session) (*models.Session, error) {
	sessionJSON, err := json.Marshal(session)
	if err != nil {
		return nil, fmt.Errorf("failed to marshal session: %w", err)
	}

	partitionKey := azcosmos.NewPartitionKeyString(session.SessionId)
	itemResponse, err := repository.container.ReplaceItem(ctx, partitionKey, session.SessionId, sessionJSON, nil)
	if err != nil {
		return nil, fmt.Errorf("failed to update session: %w", err)
	}

	var updatedSession models.Session

	if err := json.Unmarshal(itemResponse.Value, &updatedSession); err != nil {
		return nil, fmt.Errorf("failed to unmarshal session: %w", err)
	}

	return &updatedSession, nil
}

func (repository *SessionRepository) Delete(ctx context.Context, id string) (string, error) {
	partitionKey := azcosmos.NewPartitionKeyString(id)

	_, err := repository.container.DeleteItem(
		ctx,
		partitionKey,
		id,
		nil)

	if err != nil {
		return "", fmt.Errorf("failed to delete session: %w", err)
	}

	return id, nil
}
