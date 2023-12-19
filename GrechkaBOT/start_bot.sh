while true; do
	if ! pgrep -f 'GrechkaBOT' >/dev/null; then
		sh ./_watchdog.sh
		echo "Starting bot.."
	fi
	sleep 30
done
