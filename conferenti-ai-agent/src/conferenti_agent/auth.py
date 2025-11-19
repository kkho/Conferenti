from functools import lru_cache
import os
from conferenti_agent.config import get_settings
from fastapi import Header, HTTPException, Security, requests, status
from fastapi.security import HTTPBearer
from jose import jwt, JWTError
import requests


security = HTTPBearer()

AUTH0_DOMAIN = os.getenv("AUTH0_DOMAIN", "your-auth0-domain")
AUTH0_AUDIENCE = os.getenv("AUTH0_AUDIENCE", "https://conferenti.com/api/")
ALGORITHMS = ["RS256"]


@lru_cache()
def get_jwks():
    """Cache JWKS (public keys) from Auth0"""
    response = requests.get(f"https://{AUTH0_DOMAIN}/.well-known/jwks.json")
    return response.json()


async def verify_token(credentials=Security(security)) -> dict:
    """Validate JWT token"""
    settings = get_settings()

    if settings.disable_auth:
        return {
            "sub": "local-dev",
            "scope": "ai:suggest:speakers ai:suggest:sessions ai:chat",
        }

    token = credentials.credentials

    try:
        jwks = get_jwks()
        unverified_header = jwt.get_unverified_header(token)

        rsa_key = {}
        for key in jwks["keys"]:
            if key["kid"] == unverified_header["kid"]:
                rsa_key = {
                    "kty": key["kty"],
                    "kid": key["kid"],
                    "use": key["use"],
                    "n": key["n"],
                    "e": key["e"],
                }
        if not rsa_key:
            raise HTTPException(
                status_code=401, detail="Unable to find appropriate key"
            )

        # Verify and decode token
        payload = jwt.decode(
            token,
            rsa_key,
            algorithms=ALGORITHMS,
            audience=AUTH0_AUDIENCE,
            issuer=f"https://{AUTH0_DOMAIN}/",
        )

        return payload

    except JWTError as e:
        raise HTTPException(status_code=401, detail=f"Invalid token: {str(e)}")
    except Exception as e:
        raise HTTPException(
            status_code=401, detail=f"Token validation failed: {str(e)}"
        )


def require_scope(required_scope: str):
    """
    Dependency to check if token has required scope.
    Usage: dependencies=[Depends(require_scope("ai:suggest:speakers"))]
    """

    def scope_checker(token: dict = Security(verify_token)) -> dict:
        token_scopes = token.get("scope", "").split()

        if required_scope not in token_scopes:
            raise HTTPException(
                status_code=403,
                detail=f"Insufficient permissions. Required {required_scope}",
            )

        return token

    return scope_checker
