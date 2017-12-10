package pso2

import (
	"context"
	"fmt"
	"net/http"
	"strconv"

	"github.com/favclip/ucon"
)

func EmagView(w http.ResponseWriter, r *http.Request, ctx context.Context) {
	HtmlHeder(w)

	params := ctx.Value(ucon.PathParameterKey).(map[string]string)
	date := params["date"]

	l := CnvEmaList(GetEmaListV2(r, date))

	fmt.Fprintln(w, date[0:4], "年", date[4:6], "月", date[6:8], "日")
	fmt.Fprintln(w, "の緊急クエストは<br>")

	for _, v := range l {
		fmt.Fprintln(w, v, "<br>")
	}

	i, err := strconv.Atoi(date)
	if err != nil {
		return
	}
	tom := i + 1
	yes := i - 1
	fmt.Fprintln(w, fmt.Sprintf("<a href = '/pso2/emag_list/%d'>昨日</a>", yes))
	fmt.Fprintln(w, fmt.Sprintf("<a href = '/pso2/emag_list/%d'>明日</a>", tom))
	HtmlFooter(w)
}
