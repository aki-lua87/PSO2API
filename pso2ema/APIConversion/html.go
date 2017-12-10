package pso2

import (
	"fmt"
	"net/http"
)

func HtmlHeder(w http.ResponseWriter) {
	fmt.Fprintln(w, "<!DOCTYPE html><html><body><head><meta name='viewport' content='width=device-width, initial-scale=1'><title>akakitune87.net</title></head>")

}

func HtmlFooter(w http.ResponseWriter) {
	fmt.Fprintln(w, "</body></html>")
}
