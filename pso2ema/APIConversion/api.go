package pso2

import (
	"context"
	"encoding/json"
	"net/http"
)

func decodeBody(resp *http.Request, out interface{}) error {
	defer resp.Body.Close()
	decoder := json.NewDecoder(resp.Body)
	return decoder.Decode(out)
}

func GetPSO2emaV4(w http.ResponseWriter, r *http.Request, ctx context.Context) {
	type eventRequest struct {
		EventDate string `json:"EventDate"`
		EventType string `json:"EventType"`
	}
	var request eventRequest
	if err := decodeBody(r, &request); err != nil {
		w.WriteHeader(418)
		return
	}

	respons, err := GetEmaListV4(r, request.EventDate, request.EventType)
	if err != nil {
		w.Header().Set("Content-Type", "application/json")
		w.WriteHeader(500)
		return
	}

	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(200)
	json.NewEncoder(w).Encode(respons)

	return
}
