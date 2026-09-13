#!/usr/bin/env python3
"""Fail CI when measured line coverage drops below a threshold.

Usage: python3 eng/check_coverage.py <threshold-percent>
The 100% production-coverage target is the release requirement; this gate
prevents regressions from a documented baseline while the remaining gaps close.
"""
import glob
import os
import re
import sys


def main() -> int:
    threshold = float(sys.argv[1]) if len(sys.argv) > 1 else 60.0
    reports = sorted(
        glob.glob("test/Plotter.PropertyTests/TestResults/*/coverage.cobertura.xml"),
        key=os.path.getmtime,
    )
    if not reports:
        print("No coverage report found. Ensure 'dotnet test --settings coverage.runsettings' ran.")
        return 1
    text = open(reports[-1], encoding="utf-8").read()
    match = re.search(r'line-rate="([0-9.]+)"', text)
    if not match:
        print("No line-rate found in the coverage report.")
        return 1
    line_rate = float(match.group(1)) * 100
    print(f"Line coverage: {line_rate:.1f}% (threshold {threshold:.1f}%)")
    return 0 if line_rate >= threshold else 1


if __name__ == "__main__":
    sys.exit(main())
