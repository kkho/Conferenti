package repositories

import (
	"context"
	"encoding/json"
	"fmt"

	"github.com/Azure/azure-sdk-for-go/sdk/data/azcosmos"
	"github.com/kkho/conferenti/conferenti-admin-api/models"
)

type SpeakerRepository struct {
	container *azcosmos.ContainerClient
}

func NewSpeakerRepository(container *azcosmos.ContainerClient) *SpeakerRepository {
	return &SpeakerRepository{
		container: container,
	}
}

func (repository *SpeakerRepository) Create(ctx context.Context, speaker *models.Speaker) error {
	speakerJSON, err := json.Marshal(speaker)

	if err != nil {
		return fmt.Errorf("failed to marshal speaker: %w", err)
	}

	partitionKey := azcosmos.NewPartitionKeyString(speaker.Id)

	_, err = repository.container.CreateItem(ctx, partitionKey, speakerJSON, nil)

	if err != nil {
		return fmt.Errorf("failed to create speaker: %w", err)
	}

	return nil
}

func (repository *SpeakerRepository) GetAll(ctx context.Context) ([]*models.Speaker, error) {
	query := "SELECT * FROM c"
	queryOptions := &azcosmos.QueryOptions{}

	// Use empty partition key for cross-partition query
	queryPager := repository.container.NewQueryItemsPager(
		query,
		azcosmos.PartitionKey{},
		queryOptions)

	speakers := make([]*models.Speaker, 0)

	for queryPager.More() {
		queryResponse, err := queryPager.NextPage(ctx)
		if err != nil {
			return nil, fmt.Errorf("failed to query speakers: %w", err)
		}

		for _, item := range queryResponse.Items {
			var speaker models.Speaker
			if err := json.Unmarshal(item, &speaker); err != nil {
				return nil, fmt.Errorf("failed to unmarshal speaker: %w", err)
			}
			speakers = append(speakers, &speaker)
		}

	}

	return speakers, nil
}

func (repository *SpeakerRepository) Update(ctx context.Context, speaker *models.Speaker) (*models.Speaker, error) {
	speakerJSON, err := json.Marshal(speaker)
	if err != nil {
		return nil, fmt.Errorf("failed to marshal speaker: %w", err)
	}

	partitionKey := azcosmos.NewPartitionKeyString(speaker.Id)
	itemResponse, err := repository.container.ReplaceItem(ctx, partitionKey, speaker.Id, speakerJSON, nil)
	if err != nil {
		return nil, fmt.Errorf("failed to update speaker: %w", err)
	}

	var updatedSpeaker models.Speaker

	if err := json.Unmarshal(itemResponse.Value, &updatedSpeaker); err != nil {
		return nil, fmt.Errorf("failed to unmarshal speaker: %w", err)
	}

	return &updatedSpeaker, nil
}

func (repository *SpeakerRepository) Delete(ctx context.Context, id string) (string, error) {
	partitionKey := azcosmos.NewPartitionKeyString(id)

	_, err := repository.container.DeleteItem(
		ctx,
		partitionKey,
		id,
		nil)

	if err != nil {
		return "", fmt.Errorf("failed to delete speaker: %w", err)
	}

	return id, nil
}
