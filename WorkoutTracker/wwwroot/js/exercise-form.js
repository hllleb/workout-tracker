'use strict';

(function () {
    const form = document.getElementById('exercise-form');
    const typeInputs = document.querySelectorAll('input[name="ExerciseType"]');
    const strengthSection = document.getElementById('strength-fields');
    const cardioSection = document.getElementById('cardio-fields');
    const speedSection = document.getElementById('speed-field');
    const setsInput = document.getElementById('sets-input');
    const repsContainer = document.getElementById('reps-container');
    const calPreview = document.getElementById('cal-preview');
    const nameInput = document.querySelector('input[name="Name"]');
    const durInput = document.querySelector('input[name="DurationMinutes"]');
    const distInput = document.querySelector('input[name="DistanceKm"]');
    const speedInput = document.querySelector('input[name="SpeedKmh"]');

    const speedExercises = (form?.dataset.speedExercises || '')
        .split(',')
        .map(s => s.trim().toLowerCase())
        .filter(Boolean);

    let lastMetricField = null;

    function getType() {
        for (const r of typeInputs) {
            if (r.checked) return r.value;
        }
        return 'Strength';
    }

    function supportsSpeed() {
        const name = (nameInput?.value || '').trim().toLowerCase();
        return speedExercises.includes(name);
    }

    function parseNum(input) {
        const value = parseFloat(input?.value || '');
        return Number.isFinite(value) && value > 0 ? value : null;
    }

    function setInputValue(input, value, decimals) {
        if (!input || value === null) return;
        input.value = value.toFixed(decimals);
    }

    function syncSpeedMetrics(changedField) {
        if (!supportsSpeed()) return;

        const duration = parseNum(durInput);
        const distance = parseNum(distInput);
        const speed = parseNum(speedInput);
        const field = changedField || lastMetricField;

        if (field === 'duration' || field === 'distance') {
            if (duration && distance) {
                setInputValue(speedInput, distance / (duration / 60), 1);
            }
            return;
        }

        if (field === 'speed') {
            if (speed && duration) {
                setInputValue(distInput, speed * (duration / 60), 2);
            } else if (speed && distance) {
                setInputValue(durInput, distance / speed * 60, 1);
            }
            return;
        }

        if (duration && distance) {
            setInputValue(speedInput, distance / (duration / 60), 1);
        } else if (speed && duration) {
            setInputValue(distInput, speed * (duration / 60), 2);
        } else if (speed && distance) {
            setInputValue(durInput, distance / speed * 60, 1);
        }
    }

    function toggleSpeedField() {
        if (!speedSection) return;
        const show = getType() === 'Cardio' && supportsSpeed();
        speedSection.style.display = show ? '' : 'none';
        if (!show && speedInput) {
            speedInput.value = '';
        }
    }

    function toggleType() {
        const isStrength = getType() === 'Strength';
        strengthSection.style.display = isStrength ? '' : 'none';
        cardioSection.style.display = isStrength ? 'none' : '';
        toggleSpeedField();
        updateCalPreview();
    }

    function buildRepsFields() {
        const count = parseInt(setsInput.value, 10) || 1;
        const existing = Array.from(repsContainer.querySelectorAll('input[type=number]')).map(i => parseInt(i.value, 10) || 10);
        repsContainer.innerHTML = '';
        for (let i = 0; i < count; i++) {
            const val = existing[i] ?? (existing[existing.length - 1] ?? 10);
            const div = document.createElement('div');
            div.className = 'reps-set-group text-center';
            div.style.width = '90px';
            div.innerHTML = `<label class="form-label small text-muted mb-1">Set ${i + 1}</label>
                <input type="number" name="RepsPerSetValues[${i}]" value="${val}"
                       class="form-control text-center" min="1" max="200" />`;
            repsContainer.appendChild(div);
        }
        updateCalPreview();
    }

    function updateCalPreview() {
        if (!calPreview) return;

        const type = getType();
        if (type === 'Cardio') {
            const dur = parseFloat(durInput?.value || '0');
            const dist = parseFloat(distInput?.value || '0');
            if (dur <= 0) { calPreview.textContent = '—'; return; }
            const met = dist > 0 ? 8.5 : 6.0;
            const cal = Math.round(met * 75 * dur / 60);
            calPreview.textContent = `~${cal} kcal`;
        } else {
            const sets = parseInt(setsInput?.value || '1', 10);
            const reps = Array.from(repsContainer?.querySelectorAll('input[type=number]') || []).map(i => parseInt(i.value, 10) || 10);
            const avgReps = reps.length > 0 ? reps.reduce((a, b) => a + b, 0) / reps.length : 10;
            const weight = parseFloat(document.querySelector('input[name="WeightKg"]')?.value || '20');
            const cal = Math.round(sets * avgReps * (0.35 + weight * 0.015));
            calPreview.textContent = cal > 0 ? `~${cal} kcal` : '—';
        }
    }

    function onMetricInput(field) {
        lastMetricField = field;
        syncSpeedMetrics(field);
        updateCalPreview();
    }

    typeInputs.forEach(r => r.addEventListener('change', toggleType));
    setsInput?.addEventListener('input', buildRepsFields);
    repsContainer?.addEventListener('input', updateCalPreview);
    nameInput?.addEventListener('input', () => {
        toggleSpeedField();
        syncSpeedMetrics();
    });
    nameInput?.addEventListener('change', () => {
        toggleSpeedField();
        syncSpeedMetrics();
    });

    durInput?.addEventListener('input', () => onMetricInput('duration'));
    distInput?.addEventListener('input', () => onMetricInput('distance'));
    speedInput?.addEventListener('input', () => onMetricInput('speed'));

    const weightInput = document.querySelector('input[name="WeightKg"]');
    weightInput?.addEventListener('input', updateCalPreview);

    toggleType();
    syncSpeedMetrics();
    updateCalPreview();
})();
