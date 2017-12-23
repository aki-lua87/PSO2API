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
		w.WriteHeader(418)
		return
	}

	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(200)
	json.NewEncoder(w).Encode(respons)

	return
}

func GetPSO2emaV3(w http.ResponseWriter, r *http.Request, ctx context.Context) {
	type eventRequest struct {
		EventDate string `json:"EvantDate"`
		EventType string `json:"EventType"`
	}
	var request eventRequest
	if err := decodeBody(r, &request); err != nil {
		w.WriteHeader(418)
		return
	}

	respons := GetEmaListV3(r, request.EventDate, request.EventType)

	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(200)
	json.NewEncoder(w).Encode(respons)

	return
}

func GetPSO2emaV2(w http.ResponseWriter, r *http.Request) {

	var request string
	if err := decodeBody(r, &request); err != nil {
		w.WriteHeader(418)
		return
	}

	respons := GetEmaListV2(r, request)

	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(200)
	json.NewEncoder(w).Encode(respons)

	return
}

func GetPSO2emaV1(w http.ResponseWriter, r *http.Request) {

	if r.Method != "POST" {
		w.WriteHeader(418)
		return
	}

	var request string
	if err := decodeBody(r, &request); err != nil {
		w.WriteHeader(418)
		return
	}

	respons := GetEmaListV1(r, request)

	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(200)
	json.NewEncoder(w).Encode(respons)

	return
}
