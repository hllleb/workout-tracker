'use strict';

(function () {
    const typeInputs = document.querySelectorAll('input[name="ExerciseType"]');
    const strengthSection = document.getElementById('strength-fields');
    const cardioSection = document.getElementById('cardio-fields');
    const setsInput = document.getElementById('sets-input');
    const repsContainer = document.getElementById('reps-container');
    const calPreview = document.getElementById('cal-preview');

    function getType() {
        for (const r of typeInputs) {
            if (r.checked) return r.value;
        }
        return 'Strength';
    }

    function toggleType() {
        const isStrength = getType() === 'Strength';
        strengthSection.style.display = isStrength ? '' : 'none';
        cardioSection.style.display = isStrength ? 'none' : '';
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
            const dur = parseFloat(document.querySelector('input[name="DurationMinutes"]')?.value || '0');
            const dist = parseFloat(document.querySelector('input[name="DistanceKm"]')?.value || '0');
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

    // Attach events
    typeInputs.forEach(r => r.addEventListener('change', toggleType));
    setsInput?.addEventListener('input', buildRepsFields);
    repsContainer?.addEventListener('input', updateCalPreview);

    const durInput = document.querySelector('input[name="DurationMinutes"]');
    const distInput = document.querySelector('input[name="DistanceKm"]');
    durInput?.addEventListener('input', updateCalPreview);
    distInput?.addEventListener('input', updateCalPreview);

    const weightInput = document.querySelector('input[name="WeightKg"]');
    weightInput?.addEventListener('input', updateCalPreview);

    // Init
    toggleType();
    updateCalPreview();
})();
