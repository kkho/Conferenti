package handlers

import (
	"encoding/json"
	"net/http"
	"time"

	"github.com/kkho/conferenti/conferenti-admin-api/helpers"
	"github.com/kkho/conferenti/conferenti-admin-api/models"
	"github.com/kkho/conferenti/conferenti-admin-api/models/dto"
	"github.com/kkho/conferenti/conferenti-admin-api/services"
)

type SpeakerHandler struct {
	service *services.SpeakerService
}

func NewSpeakerHandler(service *services.SpeakerService) *SpeakerHandler {
	return &SpeakerHandler{service: service}
}

// GetSpeakers handles GET /api/v1/speakers
func (h *SpeakerHandler) GetSpeakers(w http.ResponseWriter, r *http.Request) {
	speakers, err := h.service.GetAll(r.Context())

	if err != nil {
		helpers.RespondError(w, http.StatusBadRequest, err.Error())
		return
	}

	response := dto.ApiResponse{
		Success: true,
		Data:    speakers,
	}

	helpers.RespondJSON(w, http.StatusOK, response)
}

// CreateSpeaker handles POST /api/v1/speakers
func (h *SpeakerHandler) CreateSpeaker(w http.ResponseWriter, r *http.Request) {
	var req dto.SpeakerRequest

	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		response := dto.ErrorResponse{
			Success: false,
			Error:   "Invalid request payload",
			Code:    http.StatusBadRequest,
		}

		helpers.RespondJSON(w, http.StatusBadRequest, response)
		return
	}

	speaker := &models.Speaker{
		Id:        req.Id,
		Name:      req.Name,
		Email:     req.Email,
		Bio:       req.Bio,
		Position:  req.Position,
		Company:   req.Company,
		PhotoUrl:  req.PhotoURL,
		CreatedAt: time.Now(),
		UpdatedAt: time.Now(),
	}

	created := h.service.Create(r.Context(), speaker)

	response := dto.ApiResponse{
		Success: true,
		Message: "Speaker created successfully",
		Data:    created,
	}
	helpers.RespondJSON(w, http.StatusCreated, response)
}
