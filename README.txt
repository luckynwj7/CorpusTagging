# 문장을 나누는 프로그램인 KoreanSentenceSpliter를 사용하고 진행하세요. (파이썬으로 제작, kss라이브러리 사용)

# 문장 내에 절대 들어가면 안되는 항목
 >> ||ExceptionArea||
 >> ||SentSplitArea||
 >> ||TempSentSplitArea||
# 위 세 가지 경우는 문자열 처리를 할 때 임시로 넣었다 삭제하게 되있습니다.
# 해당 문자가 있다면 다른 문자로 변환 후 처리해주세요.
# ||SentSplitArea|| 같은 경우에는 KoreanSentenceSpliter를 사용하면 나오는데, 이 경우에는 문제가 없습니다.

# 문장시작을 부여하는 경우, csv파일에만 그 형식이 저장되어 있고 UI가 실시간으로 적용되지 않습니다.
# UI를 다시 갱신시키려고 한다면 프로그램을 닫았다 다시 실행해주세요.