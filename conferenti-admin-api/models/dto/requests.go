package dto

type SpeakerRequest struct {
	Id       string `json:"id" validate:"required"`
	Name     string `json:"name" validate:"required"`
	Email    string `json:"email" validate:"required,email"`
	Bio      string `json:"bio"`
	Position string `json:"position"`
	Company  string `json:"company"`
	PhotoURL string `json:"photoUrl"`
}

type SessionRequest struct {
	SessionId   string   `json:"id" bson:"id"`
	Title       string   `json:"title" bson:"title"`
	Slug        string   `json:"slug" bson:"slug"`
	Tags        []string `json:"tags" bson:"tags"`
	Description string   `json:"description" bson:"description"`
	Room        string   `json:"room" bson:"room"`
	Level       string   `json:"level" bson:"level"` // beginner, intermediate, advanced
	Format      string   `json:"format" bson:"format"`
	Language    string   `json:"language" bson:"language"`
	SpeakerIds  []string `json:"speakerIds" bson:"speakerIds"` // Lecture, Workshop, Panel, Keynote, Presentation
}
