package pso2

// func PSO2ema(w http.ResponseWriter, r *http.Request, ctx context.Context) {
// 	t := time.Now().Add(time.Hour * 9)

// 	getKey := fmt.Sprintf("%d%02d%02d", t.Year(), int(t.Month()), t.Day())
// 	l := CnvEmaList(GetEmaListV2(r, getKey))

// 	var postText string
// 	postText = fmt.Sprintln("今日の緊急クエストは")

// 	for _, v := range l {
// 		postText = postText + v + " \n"
// 	}
// 	fmt.Fprintln(w, postText)

// 	SlackPost(postText, urlfetch.Client(ctx))
// }
