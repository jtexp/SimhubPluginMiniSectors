#!/bin/bash
# check-build.sh - Monitor GitHub Actions build status with in-place updates

set -e

# Colors
GREEN='\033[0;32m'
YELLOW='\033[0;33m'
RED='\033[0;31m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Get the run ID (latest or specified)
RUN_ID=${1:-$(gh run list --limit 1 --json databaseId --jq '.[0].databaseId')}

if [ -z "$RUN_ID" ]; then
    echo "No runs found"
    exit 1
fi

echo "Monitoring run: $RUN_ID"
echo ""

while true; do
    # Get run status and conclusion
    status=$(gh run view "$RUN_ID" --json status --jq '.status' 2>/dev/null)
    conclusion=$(gh run view "$RUN_ID" --json conclusion --jq '.conclusion' 2>/dev/null)

    # Get job name (in_progress first, then completed)
    job_name=$(gh run view "$RUN_ID" --json jobs --jq '
        ([.jobs[] | select(.status == "in_progress")] | if length > 0 then .[0] else null end) //
        ([.jobs[] | select(.status == "completed")] | last) | .name
    ' 2>/dev/null)

    # Get current step info with timestamps (in_progress first, then last completed)
    step_info=$(gh run view "$RUN_ID" --json jobs --jq '
        ([.jobs[].steps[] | select(.status == "in_progress")] | if length > 0 then .[0] else null end) //
        ([.jobs[].steps[] | select(.status == "completed")] | last) |
        "\(.status)|\(.name)|\(.startedAt)|\(.completedAt // "null")"
    ' 2>/dev/null)

    step_status=$(echo "$step_info" | cut -d'|' -f1)
    current_step=$(echo "$step_info" | cut -d'|' -f2)
    started_at=$(echo "$step_info" | cut -d'|' -f3)
    completed_at=$(echo "$step_info" | cut -d'|' -f4)

    # Calculate actual step duration from GitHub timestamps
    elapsed_str="0s"
    # Validate timestamp looks like ISO format (starts with 20)
    if [ -n "$started_at" ] && [ "$started_at" != "null" ] && [[ "$started_at" == 20* ]]; then
        start_epoch=$(date -d "$started_at" +%s 2>/dev/null || echo "")
        if [ -n "$start_epoch" ] && [ "$start_epoch" -gt 0 ] 2>/dev/null; then
            if [ "$completed_at" != "null" ] && [ -n "$completed_at" ] && [[ "$completed_at" != 0001* ]]; then
                # Step completed - calculate from startedAt to completedAt
                end_epoch=$(date -d "$completed_at" +%s 2>/dev/null || echo "")
            else
                # Step in progress - calculate from startedAt to now
                end_epoch=$(date +%s)
            fi
            if [ -n "$end_epoch" ]; then
                elapsed=$((end_epoch - start_epoch))
                if [ $elapsed -ge 60 ]; then
                    mins=$((elapsed / 60))
                    secs=$((elapsed % 60))
                    elapsed_str="${mins}m ${secs}s"
                else
                    elapsed_str="${elapsed}s"
                fi
            fi
        fi
    fi

    # Format status with color
    case "$status" in
        "in_progress")
            status_color="${YELLOW}in_progress${NC}"
            ;;
        "completed")
            if [ "$conclusion" = "success" ]; then
                status_color="${GREEN}success${NC}"
            else
                status_color="${RED}${conclusion}${NC}"
            fi
            ;;
        *)
            status_color="$status"
            ;;
    esac

    # Format step status indicator
    case "$step_status" in
        "in_progress")
            step_indicator="⏳"
            ;;
        "completed")
            step_indicator="✓"
            ;;
        *)
            step_indicator="·"
            ;;
    esac

    # Clear line and print status (using \r to return to start of line)
    printf "\r\033[K"
    printf "Status: ${status_color} | Job: ${CYAN}%s${NC} | Step: %s %s (${YELLOW}%s${NC})" \
        "$job_name" "$step_indicator" "$current_step" "$elapsed_str"

    # Exit conditions
    if [ "$status" = "completed" ]; then
        echo ""  # New line after completion
        echo ""
        if [ "$conclusion" = "success" ]; then
            echo -e "${GREEN}Build completed successfully!${NC}"
        else
            echo -e "${RED}Build failed with conclusion: $conclusion${NC}"
            exit 1
        fi
        break
    fi

    sleep 2
done
