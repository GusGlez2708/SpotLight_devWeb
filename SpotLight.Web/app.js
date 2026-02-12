// ---------------------------------------------------------
// CONFIGURACIÓN
// ---------------------------------------------------------
const API_URL = "/api";

// ---------------------------------------------------------
// CONSTANTES & ESTADO
// ---------------------------------------------------------
const PREDEFINED_TECHS = [
    "React", "Angular", "Vue.js", "Svelte", "Node.js", ".NET Core",
    "Python", "Django", "Flask", "Java", "Spring Boot", "Kotlin", "Swift",
    "Flutter", "Dart", "SQL Server", "PostgreSQL", "MongoDB", "Redis",
    "Docker", "Kubernetes", "AWS", "Azure", "Google Cloud", "TensorFlow",
    "PyTorch", "OpenCV", "Arduino", "Raspberry Pi", "IoT"
];

const PREDEFINED_PLATFORMS = ["Web", "Móvil", "Web y Móvil"];

const appState = {
    techs: { create: [], edit: [] }
};

// Inicializar Datalists
document.addEventListener('DOMContentLoaded', () => {
    const options = PREDEFINED_TECHS.map(t => `<option value="${t}">`).join('');
    document.getElementById('techOptions').innerHTML = options;
    document.getElementById('editTechOptions').innerHTML = options;

    // Inicializar inputs de miembros (al menos uno vacío)
    addMemberInput('membersList');
});

// ---------------------------------------------------------
// UTILIDADES — Toast Notifications
// ---------------------------------------------------------
function showToast(message, type = 'success') {
    const container = document.getElementById('toastContainer');
    const toast = document.createElement('div');
    toast.className = `toast ${type}`;
    toast.innerHTML = `
        <i class="fa-solid ${type === 'success' ? 'fa-circle-check' : 'fa-circle-xmark'}"></i>
        <span class="toast-text">${message}</span>
    `;
    container.appendChild(toast);
    setTimeout(() => toast.remove(), 3000);
}

// ---------------------------------------------------------
// UTILIDADES — Modales
// ---------------------------------------------------------
function abrirModal(id) {
    document.getElementById(id).classList.add('show');
}

function cerrarModal(id) {
    document.getElementById(id).classList.remove('show');
}

// Cerrar modal al hacer clic fuera
document.addEventListener('click', (e) => {
    if (e.target.classList.contains('modal-overlay')) {
        e.target.classList.remove('show');
    }
});

// ---------------------------------------------------------
// UTILIDAD: Recalcular Estadísticas (Sync)
// ---------------------------------------------------------
async function recalcularEstadisticas() {
    const btn = document.querySelector('.btn-refresh[title="Recalcular estadísticas de IA"]');
    if (!btn) return;

    const originalContent = btn.innerHTML;

    try {
        btn.innerHTML = '<i class="fa-solid fa-spin fa-spinner"></i> Sincronizando...';
        btn.disabled = true;

        const response = await fetch(`${API_URL}/projects/recalculate-stats`, {
            method: 'POST'
        });

        if (!response.ok) throw new Error('Error al sincronizar');

        const data = await response.json();
        showToast(data.message || 'Datos sincronizados correctamente', 'success');

        // Recargar la lista de proyectos para ver los cambios
        await cargarProyectos();

    } catch (error) {
        console.error(error);
        showToast('Error al sincronizar datos', 'error');
    } finally {
        btn.innerHTML = originalContent;
        btn.disabled = false;
    }
}

// ---------------------------------------------------------
// 1. FUNCIÓN: Cargar Proyectos (GET)
// ---------------------------------------------------------
async function cargarProyectos() {
    const contenedor = document.getElementById('projectsList');

    contenedor.innerHTML = `
        <div class="loading-state">
            <div class="loading-spinner"></div>
            <p>Cargando proyectos...</p>
        </div>
    `;

    try {
        console.log("Conectando a:", API_URL);
        const respuesta = await fetch(`${API_URL}/projects`);

        if (!respuesta.ok) {
            throw new Error(`Error del servidor: ${respuesta.status}`);
        }

        const proyectos = await respuesta.json();
        console.log("Proyectos recibidos:", proyectos);

        // Actualizar badge API como conectada
        const badge = document.getElementById('apiBadge');
        badge.textContent = `${proyectos.length} proyecto${proyectos.length !== 1 ? 's' : ''}`;
        badge.style.background = '';
        badge.style.color = '';
        badge.style.borderColor = '';

        contenedor.innerHTML = '';

        if (proyectos.length === 0) {
            contenedor.innerHTML = `
                <div class="empty-state">
                    <i class="fa-solid fa-folder-open"></i>
                    <p class="empty-title">No hay proyectos aún</p>
                    <p>¡Crea el primer proyecto usando el formulario!</p>
                </div>
            `;
            return;
        }

        // Renderizar tarjetas con delay de animación
        proyectos.forEach((p, index) => {
            const status = p.status || 'activo';
            const isActive = status === 'activo';

            // Extract Platform from Techs[0]
            let platform = 'Otro';
            let displayTechs = p.technologies || [];
            if (displayTechs.length > 0 && PREDEFINED_PLATFORMS.includes(displayTechs[0])) {
                platform = displayTechs[0];
                displayTechs = displayTechs.slice(1);
            }

            // Determine badge class
            let badgeClass = 'otro';
            if (platform === 'Web') badgeClass = 'web';
            else if (platform === 'Móvil') badgeClass = 'movil';
            else if (platform === 'Web y Móvil') badgeClass = 'both';

            const card = document.createElement('div');
            card.className = `project-card status-${status}`;
            card.style.animationDelay = `${index * 0.08}s`;

            card.innerHTML = `
                <div class="card-top">
                    <div class="card-info">
                        <div class="card-badges">
                            <span class="badge-category">${p.category}</span>
                            <span class="badge-platform ${badgeClass}">
                                <i class="fa-solid fa-laptop-code"></i> ${platform}
                            </span>
                            <span class="badge-status ${status}">
                                <span class="badge-status-dot"></span>
                                ${isActive ? 'Activo' : 'Desactivado'}
                            </span>
                        </div>
                        <h3 class="card-title">${p.title}</h3>
                        <p class="card-desc">${p.description || 'Sin descripción'}</p>
                    </div>
                    <div class="score-box">
                        <div class="score-value">
                            <i class="fa-solid fa-brain"></i>
                            ${p.stats ? p.stats.puntuacionFactibilidad || p.stats.puntuacion_factibilidad || 0 : 0}%
                        </div>
                        <div class="score-label">${p.stats ? p.stats.totalEvaluaciones || 0 : 0} Eval.</div>
                    </div>
                </div>
                
                <!-- Members & Techs Display -->
                <div class="card-details" style="margin-bottom: 12px;">
                    ${(p.members && p.members.length > 0) ? `
                        <div class="members-display" style="font-size: 0.8rem; color: var(--text-muted); margin-bottom: 6px;">
                            <i class="fa-solid fa-user-group"></i> ${p.members.join(', ')}
                        </div>
                    ` : ''}
                    
                    ${(displayTechs && displayTechs.length > 0) ? `
                        <div class="tech-tags-list" style="gap: 4px;">
                            ${displayTechs.slice(0, 5).map(t =>
                `<span class="tech-tag" style="padding: 2px 8px; font-size: 0.7rem;">${t}</span>`
            ).join('')}
                            ${displayTechs.length > 5 ? `<span class="tech-tag" style="padding: 2px 6px; font-size: 0.7rem;">+${displayTechs.length - 5}</span>` : ''}
                        </div>
                    ` : ''}
                </div>

                <div class="card-footer">
                    <span class="card-meta">
                        <i class="fa-solid fa-users"></i> Equipo #${p.equipoNumero || p.equipo_numero || 0}
                    </span>
                    <a href="${p.videoUrl}" target="_blank" class="card-link">
                        <i class="fa-solid fa-play"></i> Ver Video
                    </a>
                </div>
                <div class="card-actions">
                    <button class="btn-action ${isActive ? 'btn-toggle-active' : 'btn-toggle-inactive'}"
                            onclick="toggleStatus('${p.id}', '${status}')">
                        <i class="fa-solid ${isActive ? 'fa-pause' : 'fa-play'}"></i>
                        ${isActive ? 'Desactivar' : 'Activar'}
                    </button>
                    <button class="btn-action btn-edit"
                            onclick='abrirEdicion(${JSON.stringify(p).replace(/'/g, "&#39;")})'
                            ${isActive ? 'disabled title="Desactiva el proyecto para editar"' : ''}>
                        <i class="fa-solid fa-pen"></i> Editar
                    </button>
                    <button class="btn-action btn-delete"
                            onclick="confirmarEliminar('${p.id}', '${(p.title || 'Sin título').replace(/'/g, "\\'")}')">
                        <i class="fa-solid fa-trash"></i> Eliminar
                    </button>
                </div>
            `;
            contenedor.appendChild(card);
        });

    } catch (error) {
        console.error("Error grave:", error);
        contenedor.innerHTML = `
            <div class="error-state">
                <i class="fa-solid fa-triangle-exclamation"></i>
                <div>
                    <strong>Error de Conexión</strong>
                    <p>Asegúrate de que la API .NET esté corriendo en el puerto 5280.</p>
                </div>
            </div>
        `;

        const badge = document.getElementById('apiBadge');
        badge.textContent = 'API Desconectada';
        badge.style.background = 'rgba(239, 68, 68, 0.12)';
        badge.style.color = '#EF4444';
        badge.style.borderColor = 'rgba(239, 68, 68, 0.2)';
    }
}

// ---------------------------------------------------------
// 2. FUNCIÓN: Crear Proyecto (POST)
// ---------------------------------------------------------
async function crearProyecto(evento) {
    evento.preventDefault();

    // Prepare Technologies (Platform + Tags)
    const platform = document.getElementById('platform').value;
    const finalTechs = [platform, ...appState.techs.create];

    const nuevoProyecto = {
        title: document.getElementById('title').value,
        category: document.getElementById('category').value,
        description: document.getElementById('desc').value,
        videoUrl: document.getElementById('videoUrl').value,
        imageUrl: document.getElementById('imageUrl').value,
        equipoNumero: parseInt(document.getElementById('teamNo').value) || 0,
        members: getMembersFromInput('membersList'),
        technologies: finalTechs,
        stats: { puntuacionFactibilidad: 0, totalEvaluaciones: 0 },
        status: "activo"
    };

    // Validación manual de campos requeridos
    limpiarErrores();
    let esValido = true;

    if (!nuevoProyecto.title.trim()) {
        mostrarErrorCampo('title', 'El título es obligatorio.');
        esValido = false;
    }
    if (!nuevoProyecto.videoUrl.trim()) {
        mostrarErrorCampo('videoUrl', 'La URL del video es obligatoria.');
        esValido = false;
    }
    if (!nuevoProyecto.imageUrl.trim()) {
        mostrarErrorCampo('imageUrl', 'La URL de la imagen es obligatoria.');
        esValido = false;
    }
    if (nuevoProyecto.equipoNumero <= 0) {
        mostrarErrorCampo('teamNo', 'El número de equipo debe ser mayor a 0.');
        esValido = false;
    }

    if (!esValido) return;

    console.log("Enviando:", nuevoProyecto);

    try {
        // limpiarErrores(); // Ya se limpió arriba

        const respuesta = await fetch(`${API_URL}/projects`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(nuevoProyecto)
        });

        if (respuesta.ok) {
            showToast('¡Proyecto creado exitosamente!', 'success');
            document.getElementById('projectForm').reset();

            // Reset dynamic fields
            document.getElementById('membersList').innerHTML = '';
            addMemberInput('membersList');
            appState.techs.create = [];
            renderTechTags('techTagsContainer', 'create');

            cargarProyectos();
        } else if (respuesta.status === 409) {
            const errorData = await respuesta.json();
            const mensaje = errorData.message || 'Error de validación';

            if (mensaje.includes('número de equipo')) {
                mostrarErrorCampo('teamNo', mensaje);
            } else if (mensaje.includes('nombre')) {
                mostrarErrorCampo('title', mensaje);
            } else {
                showToast(mensaje, 'error');
            }
        } else {
            const errorTexto = await respuesta.text();
            console.error("Error del servidor:", errorTexto); // Log detailed error
            showToast('Error al guardar: ' + errorTexto, 'error');
        }

    } catch (error) {
        console.error(error);
        showToast('Error de red. Revisa la consola (F12).', 'error');
    }
}

// ---------------------------------------------------------
// 3. FUNCIÓN: Toggle Estado (PATCH)
// ---------------------------------------------------------
async function toggleStatus(id, statusActual) {
    const nuevoStatus = statusActual === 'activo' ? 'desactivado' : 'activo';

    try {
        const respuesta = await fetch(`${API_URL}/projects/${id}/status`, {
            method: 'PATCH',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ status: nuevoStatus })
        });

        if (respuesta.ok) {
            const label = nuevoStatus === 'activo' ? 'activado' : 'desactivado';
            showToast(`Proyecto ${label} correctamente`, 'success');
            cargarProyectos();
        } else {
            const errorTexto = await respuesta.text();
            showToast('Error al cambiar estado: ' + errorTexto, 'error');
        }

    } catch (error) {
        console.error(error);
        showToast('Error de red al cambiar estado.', 'error');
    }
}

// ---------------------------------------------------------
// 4. FUNCIÓN: Abrir Modal de Edición
// ---------------------------------------------------------
function abrirEdicion(proyecto) {
    document.getElementById('editTitle').value = proyecto.title || '';
    document.getElementById('editCategory').value = proyecto.category || 'Tecnología';
    document.getElementById('editDesc').value = proyecto.description || '';
    document.getElementById('editVideoUrl').value = proyecto.videoUrl || '';
    document.getElementById('editImageUrl').value = proyecto.imageUrl || '';
    document.getElementById('editTeamNo').value = proyecto.equipoNumero || proyecto.equipo_numero || 1;

    // Poblar Miembros
    const membersList = document.getElementById('editMembersList');
    membersList.innerHTML = '';
    if (proyecto.members && proyecto.members.length > 0) {
        proyecto.members.forEach(m => addMemberInput('editMembersList', m));
    } else {
        addMemberInput('editMembersList');
    }

    // Poblar Tecnologías y Plataforma
    let techsToEdit = [...(proyecto.technologies || [])];
    let platform = 'Web'; // Default

    if (techsToEdit.length > 0 && PREDEFINED_PLATFORMS.includes(techsToEdit[0])) {
        platform = techsToEdit[0];
        techsToEdit.shift(); // Remove platform from tags
    }

    document.getElementById('editPlatform').value = platform;
    appState.techs.edit = techsToEdit;
    renderTechTags('editTechTagsContainer', 'edit');

    // Configurar el botón de guardar
    const btnGuardar = document.getElementById('btnConfirmEdit');
    btnGuardar.onclick = () => guardarEdicion(proyecto.id, proyecto);

    abrirModal('editModal');
}

// ---------------------------------------------------------
// 5. FUNCIÓN: Guardar Edición (PUT)
// ---------------------------------------------------------
async function guardarEdicion(id, proyectoOriginal) {
    // Prepare Technologies (Platform + Tags)
    const platform = document.getElementById('editPlatform').value;
    const finalTechs = [platform, ...appState.techs.edit];

    const proyectoActualizado = {
        id: id,
        title: document.getElementById('editTitle').value,
        category: document.getElementById('editCategory').value,
        description: document.getElementById('editDesc').value,
        videoUrl: document.getElementById('editVideoUrl').value,
        imageUrl: document.getElementById('editImageUrl').value,
        equipoNumero: parseInt(document.getElementById('editTeamNo').value) || 0,
        members: getMembersFromInput('editMembersList'),
        technologies: finalTechs,
        stats: proyectoOriginal.stats || { puntuacionFactibilidad: 0, totalEvaluaciones: 0 },
        status: proyectoOriginal.status || 'desactivado'
    };
    // Validación manual de campos requeridos
    limpiarErrores();
    let esValido = true;

    if (!proyectoActualizado.title.trim()) {
        mostrarErrorCampo('editTitle', 'El título es obligatorio.');
        esValido = false;
    }
    if (!proyectoActualizado.videoUrl.trim()) {
        mostrarErrorCampo('editVideoUrl', 'La URL del video es obligatoria.');
        esValido = false;
    }
    if (!proyectoActualizado.imageUrl.trim()) {
        mostrarErrorCampo('editImageUrl', 'La URL de la imagen es obligatoria.');
        esValido = false;
    }
    if (proyectoActualizado.equipoNumero <= 0) {
        mostrarErrorCampo('editTeamNo', 'El número de equipo debe ser mayor a 0.');
        esValido = false;
    }

    if (!esValido) return;

    try {
        // limpiarErrores(); // Ya se limpió arriba

        const respuesta = await fetch(`${API_URL}/projects/${id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(proyectoActualizado)
        });

        if (respuesta.ok || respuesta.status === 204) {
            showToast('Proyecto actualizado correctamente', 'success');
            cerrarModal('editModal');
            cargarProyectos();
        } else if (respuesta.status === 409) {
            const errorData = await respuesta.json();
            const mensaje = errorData.message || 'Error de validación';

            if (mensaje.includes('número de equipo')) {
                mostrarErrorCampo('editTeamNo', mensaje);
            } else if (mensaje.includes('nombre')) {
                mostrarErrorCampo('editTitle', mensaje);
            } else {
                showToast(mensaje, 'error');
            }
        } else {
            const errorTexto = await respuesta.text();
            console.error("Error del servidor (PUT):", errorTexto); // Log detailed error
            showToast('Error al actualizar: ' + errorTexto, 'error');
        }

    } catch (error) {
        console.error(error);
        showToast('Error de red al actualizar.', 'error');
    }
}

// ---------------------------------------------------------
// 6. FUNCIÓN: Confirmar Eliminación
// ---------------------------------------------------------
function confirmarEliminar(id, nombre) {
    document.getElementById('deleteProjectName').textContent = `"${nombre}"`;

    const btnConfirm = document.getElementById('btnConfirmDelete');
    btnConfirm.onclick = () => eliminarProyecto(id);

    abrirModal('deleteModal');
}

// ---------------------------------------------------------
// 7. FUNCIÓN: Eliminar Proyecto (DELETE)
// ---------------------------------------------------------
async function eliminarProyecto(id) {
    try {
        const respuesta = await fetch(`${API_URL}/projects/${id}`, {
            method: 'DELETE'
        });

        if (respuesta.ok || respuesta.status === 204) {
            showToast('Proyecto eliminado permanentemente', 'success');
            cerrarModal('deleteModal');
            cargarProyectos();
        } else {
            const errorTexto = await respuesta.text();
            showToast('Error al eliminar: ' + errorTexto, 'error');
        }

    } catch (error) {
        console.error(error);
        showToast('Error de red al eliminar.', 'error');
    }
}

// ---------------------------------------------------------
// Cargar lista automáticamente al abrir la página
// ---------------------------------------------------------
// ---------------------------------------------------------
// 8. FUNCIONES DE VALIDACIÓN VISUAL
// ---------------------------------------------------------
function mostrarErrorCampo(idCampo, mensaje) {
    const input = document.getElementById(idCampo);
    if (!input) return;

    input.classList.add('input-error');

    // Crear mensaje de error
    const errorDiv = document.createElement('div');
    errorDiv.className = 'error-message';
    errorDiv.innerText = mensaje;

    // Insertar después del input
    input.parentNode.insertBefore(errorDiv, input.nextSibling);
}

function limpiarErrores() {
    // Remover clases de error
    document.querySelectorAll('.input-error').forEach(el => el.classList.remove('input-error'));

    // Remover mensajes de error
    document.querySelectorAll('.error-message').forEach(el => el.remove());
}

// ---------------------------------------------------------
// 9. LOGICA DINAMICA (MIEMBROS & TECNOLOGIAS)
// ---------------------------------------------------------

// --- Miembros ---
function addMemberInput(containerId, value = '') {
    const container = document.getElementById(containerId);
    if (container.children.length >= 6) {
        showToast('Máximo 6 integrantes permitidos.', 'error');
        return;
    }

    const div = document.createElement('div');
    div.className = 'dynamic-input-group';
    div.innerHTML = `
        <input type="text" class="form-input member-input" placeholder="Nombre del integrante" value="${value}" required>
        <button type="button" class="btn-remove-item" onclick="removeMemberInput(this)">
            <i class="fa-solid fa-trash"></i>
        </button>
    `;
    container.appendChild(div);
}

function removeMemberInput(btn) {
    const container = btn.closest('.dynamic-list-container');
    if (container.children.length > 1) {
        btn.parentElement.remove();
    } else {
        // Si es el último, solo limpiar el value
        btn.parentElement.querySelector('input').value = '';
    }
}

function getMembersFromInput(containerId) {
    const inputs = document.querySelectorAll(`#${containerId} .member-input`);
    const members = [];
    inputs.forEach(input => {
        if (input.value.trim()) members.push(input.value.trim());
    });
    return members;
}

// --- Tecnologías ---
function addTechTag(inputId, containerId) {
    const input = document.getElementById(inputId);
    const value = input.value.trim();
    const context = containerId === 'techTagsContainer' ? 'create' : 'edit';

    if (!value) return;

    if (appState.techs[context].includes(value)) {
        showToast('Esa tecnología ya está agregada.', 'error');
        input.value = '';
        return;
    }

    appState.techs[context].push(value);
    input.value = '';
    renderTechTags(containerId, context);
}

function renderTechTags(containerId, context) {
    const container = document.getElementById(containerId);
    container.innerHTML = '';

    appState.techs[context].forEach((tech, index) => {
        const tag = document.createElement('span');
        tag.className = 'tech-tag';
        tag.innerHTML = `
            ${tech} 
            <i class="fa-solid fa-xmark" onclick="removeTechTag(${index}, '${containerId}', '${context}')"></i>
        `;
        container.appendChild(tag);
    });
}

function removeTechTag(index, containerId, context) {
    appState.techs[context].splice(index, 1);
    renderTechTags(containerId, context);
}

// Enter key support for inputs
document.addEventListener('keydown', (e) => {
    if (e.key === 'Enter') {
        if (document.activeElement.id === 'techInput') {
            e.preventDefault();
            addTechTag('techInput', 'techTagsContainer');
        }
        if (document.activeElement.id === 'editTechInput') {
            e.preventDefault();
            addTechTag('editTechInput', 'editTechTagsContainer');
        }
    }
});

// ---------------------------------------------------------
// Cargar lista automáticamente al abrir la página
// ---------------------------------------------------------
cargarProyectos();