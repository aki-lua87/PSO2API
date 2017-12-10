package pso2

import (
	"testing" // テストで使える関数・構造体が用意されているパッケージをimport
)

func TestCnvEmaList(t *testing.T) {
	temp := []EmaListV2{
		EmaListV2{
			"緊急1",
			8,
			28,
			13,
			00,
		},
		EmaListV2{
			"緊急2",
			8,
			28,
			19,
			30,
		},
	}
	actual := CnvEmaList(temp)
	expected := []string{"13:00 緊急1", "19:30 緊急2"}
	if len(actual) != len(expected) {
		t.Fatalf("failed test")
	}
	if actual[0] != expected[0] {
		t.Fatalf("failed test1")
	}
	if actual[1] != expected[1] {
		t.Fatalf("failed test2")
	}
}
