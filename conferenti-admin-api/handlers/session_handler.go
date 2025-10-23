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

type SessionHandler struct {
	service *services.SessionService
}

func NewSessionHandler(service *services.SessionService) *SessionHandler {
	return &SessionHandler{service: service}
}

// GetSessions handles GET /api/v1/sessions
func (h *SessionHandler) GetSessions(w http.ResponseWriter, r *http.Request) {
	sessions, err := h.service.GetAll(r.Context())

	if err != nil {
		helpers.RespondError(w, http.StatusBadRequest, err.Error())
		return
	}

	response := dto.ApiResponse{
		Success: true,
		Data:    sessions,
	}

	helpers.RespondJSON(w, http.StatusOK, response)
}

// CreateSession handles POST /api/v1/sessions
func (h *SessionHandler) CreateSession(w http.ResponseWriter, r *http.Request) {
	var req dto.SessionRequest

	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		response := dto.ErrorResponse{
			Success: false,
			Error:   "Invalid request payload",
			Code:    http.StatusBadRequest,
		}

		helpers.RespondJSON(w, http.StatusBadRequest, response)
		return
	}

	session := &models.Session{
		SessionId:   req.SessionId,
		Title:       req.Title,
		Description: req.Description,
		SpeakerIds:  req.SpeakerIds,
		CreatedAt:   time.Now(),
		UpdatedAt:   time.Now(),
	}

	created := h.service.Create(r.Context(), session)

	response := dto.ApiResponse{
		Success: true,
		Message: "Session created successfully",
		Data:    created,
	}

	helpers.RespondJSON(w, http.StatusCreated, response)
}
