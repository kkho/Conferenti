package models

import "time"

type Speaker struct {
	Id        string    `json:"id" bson:"id"`
	Name      string    `json:"name" bson:"name"`
	Email     string    `json:"email" bson:"email"`
	Position  string    `json:"position" bson:"position"`
	Company   string    `json:"company" bson:"company"`
	Bio       string    `json:"bio" bson:"bio"`
	PhotoUrl  string    `json:"photoUrl" bson:"photoUrl"`
	Sessions  []Session `json:"sessions" bson:"sessions"`
	CreatedAt time.Time `json:"createdAt" bson:"createdAt"`
	UpdatedAt time.Time `json:"updatedAt" bson:"updatedAt"`
}

type Session struct {
	SessionId   string    `json:"id" bson:"id"`
	Title       string    `json:"title" bson:"title"`
	Slug        string    `json:"slug" bson:"slug"`
	Tags        []string  `json:"tags" bson:"tags"`
	Description string    `json:"description" bson:"description"`
	Room        string    `json:"room" bson:"room"`
	Level       string    `json:"level" bson:"level"` // beginner, intermediate, advanced
	Format      string    `json:"format" bson:"format"`
	Language    string    `json:"language" bson:"language"`
	SpeakerIds  []string  `json:"speakerIds" bson:"speakerIds"` // Lecture, Workshop, Panel, Keynote, Presentation
	CreatedAt   time.Time `json:"createdAt" bson:"createdAt"`
	UpdatedAt   time.Time `json:"updatedAt" bson:"updatedAt"`
}
