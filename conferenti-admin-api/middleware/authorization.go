package middleware

import (
	"context"
	"encoding/json"
	"fmt"
	"net/http"
	"net/url"
	"strings"
	"time"

	jwtmiddleware "github.com/auth0/go-jwt-middleware/v2"
	"github.com/auth0/go-jwt-middleware/v2/jwks"
	"github.com/auth0/go-jwt-middleware/v2/validator"
)

type CustomClaims struct {
	Scope       string   `json:"scope"`
	Permissions []string `json:"permissions"`
}

func (c *CustomClaims) Validate(ctx context.Context) error {
	return nil
}

func NewAuth0Middleware(domain, audience string) *jwtmiddleware.JWTMiddleware {
	issuerURLStr := fmt.Sprintf("%s/", domain)
	issuerURL, err := url.Parse(issuerURLStr)
	if err != nil {
		panic(fmt.Sprintf("Invalid issuer URL: %v", err))
	}

	provider := jwks.NewCachingProvider(issuerURL, 5*time.Minute)

	jwtValidator, _ := validator.New(
		provider.KeyFunc,
		validator.RS256,
		issuerURLStr,
		[]string{audience},
		validator.WithCustomClaims(func() validator.CustomClaims {
			return &CustomClaims{}
		}),
	)

	return jwtmiddleware.New(jwtValidator.ValidateToken)
}

func RequireScope(scope string) func(http.Handler) http.Handler {
	return func(next http.Handler) http.Handler {
		return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
			context := r.Context()
			token := context.Value(jwtmiddleware.ContextKey{}).(*validator.ValidatedClaims)

			if token == nil {
				w.Header().Set("Content-Type", "application/json")
				w.WriteHeader(http.StatusUnauthorized)
				json.NewEncoder(w).Encode(map[string]interface{}{
					"success": false,
					"error":   "No valid token found",
					"code":    http.StatusUnauthorized,
				})
				return
			}

			claims := token.CustomClaims.(*CustomClaims)
			scopes := strings.Split(claims.Scope, " ")

			if len(scopes) == 0 {
				http.Error(w, "No scopes found in token", http.StatusForbidden)
				return
			}

			hasScope := false
			for _, s := range scopes {
				if s == scope {
					hasScope = true
					break
				}
			}

			if !hasScope {
				http.Error(w, fmt.Sprintf("Missing required scope: %s", scope), http.StatusForbidden)
				return
			}

			next.ServeHTTP(w, r)
		})
	}
}

func RequirePermission(permission string) func(http.Handler) http.Handler {
	return func(next http.Handler) http.Handler {
		return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
			token := r.Context().Value(jwtmiddleware.ContextKey{}).(*validator.ValidatedClaims)

			if token == nil {
				w.Header().Set("Content-Type", "application/json")
				w.WriteHeader(http.StatusUnauthorized)
				json.NewEncoder(w).Encode(map[string]interface{}{
					"success": false,
					"error":   "No valid token found",
					"code":    http.StatusUnauthorized,
				})
				return
			}

			claims := token.CustomClaims.(*CustomClaims)

			hasPermission := false
			for _, perm := range claims.Permissions {
				if perm == permission {
					hasPermission = true
					break
				}
			}

			if !hasPermission {
				w.Header().Set("Content-Type", "application/json")
				w.WriteHeader(http.StatusForbidden)
				json.NewEncoder(w).Encode(map[string]interface{}{
					"success":   false,
					"error":     fmt.Sprintf("Insufficient permissions. Required: %s", permission),
					"code":      http.StatusForbidden,
					"available": claims.Permissions,
					"scope":     claims.Scope,
				})
				return
			}

			next.ServeHTTP(w, r)
		})
	}
}
