package handlers

import (
	"net/http"

	"github.com/kkho/conferenti/conferenti-admin-api/helpers"
	"github.com/kkho/conferenti/conferenti-admin-api/models/dto"
)

type HealthHandler struct{}

func NewHealthHandler() *HealthHandler {
	return &HealthHandler{}
}

// GET /api/v1/health
func (h *HealthHandler) Check(responseWriter http.ResponseWriter, request *http.Request) {
	response := dto.ApiResponse{
		Success: true,
		Message: "Conferenti Admin Api is Healthy",
		Data: map[string]string{
			"status":  "ok",
			"version": "1.0.0",
			"service": "conferenti-admin-api",
		},
	}
	helpers.RespondJSON(responseWriter, http.StatusOK, response)
}
