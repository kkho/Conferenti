package helpers

import (
	"encoding/json"
	"net/http"

	"github.com/kkho/conferenti/conferenti-admin-api/models/dto"
)

func RespondJSON(w http.ResponseWriter, status int, payload interface{}) {
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(status)
	json.NewEncoder(w).Encode(payload)
}

func RespondError(w http.ResponseWriter, status int, message string) {
	response := dto.ErrorResponse{
		Success: false,
		Error:   message,
		Code:    status,
	}
	RespondJSON(w, status, response)
}

func RespondSuccess(w http.ResponseWriter, data interface{}, message string) {
	response := dto.ApiResponse{
		Success: true,
		Message: message,
		Data:    data,
	}
	RespondJSON(w, http.StatusOK, response)
}

func RespondCreated(w http.ResponseWriter, data interface{}, message string) {
	response := dto.ApiResponse{
		Success: true,
		Message: message,
		Data:    data,
	}
	RespondJSON(w, http.StatusCreated, response)
}
