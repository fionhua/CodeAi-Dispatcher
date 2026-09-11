# AI Coding Agent Dispatcher

[ 🇺🇸 English](README.md) | [🇨🇳 简体中文](README.zh-CN.md) | [🇭🇰 繁體中文](README.zh-TW.md) | [🇯🇵 日本語](README.ja.md) | [🇪🇸 Español](README.es.md)

> ### 🛑 Stop being the message bus between your AI coding agents.
> **Permite que tus agentes de IA se coordinen entre sí y libérate del trabajo de copiar, pegar y transferir mensajes.**
> 
> *Comunicación multiagente y despacho autónomo de tareas entre diferentes IDEs para Claude, Codex, Gemini, Copilot, Cursor, WorkBuddy y otros agentes de código.*  
> *(anteriormente CodeAi Dispatcher)*

[![License: PolyForm Noncommercial](https://img.shields.io/badge/License-PolyForm%20Noncommercial%201.0.0-purple.svg)](LICENSE)
[![Type: Source-Available](https://img.shields.io/badge/Model-Source--Available-blue.svg)](COMMERCIAL-LICENSE.md)
[![Commercial License](https://img.shields.io/badge/Commercial%20Use-License%20Required-orange.svg)](COMMERCIAL-LICENSE.md)
[![Platform](https://img.shields.io/badge/Platform-Windows%207%20%7C%2010%20%7C%2011-blue.svg)](README.md)
[![Binary Size](https://img.shields.io/badge/Binary%20Size-~55%20KB-brightgreen.svg)](bin/)

👉 **[💼 ¿Uso comercial? Solicitar Licencia Comercial / Consultas →](COMMERCIAL-LICENSE.md)**

---

## 💡 ¿Por qué lo necesitas? (The Core Value)

Cuando trabajas simultáneamente con múltiples agentes de código (Claude, Cursor, Codex, Gemini, Copilot, WorkBuddy):

- El Agente A diseña la arquitectura; tú la lees manualmente.
- La copias y pegas en el chat del Agente B para implementarla.
- El Agente B detecta un problema y pregunta; tú lo interpretas.
- Lo copias al Agente C (revisión CTO).
- Esperas, copias de vuelta y presionas al Agente A...

**Sin darte cuenta, el desarrollador humano se convierte en el conmutador de red más lento y exhausto de todo el sistema multiagente.**

AI Coding Agent Dispatcher resuelve esto de raíz:  
**Permite que los agentes en distintos IDEs se despierten mutuamente, inyecten tareas en el prompt de chat, verifiquen entregas y cierren recibos mediante archivos compartidos.**  
Los humanos se retiran del tedioso trabajo de transporte para liderar la estrategia, resolver discrepancias críticas y validar la entrega final.

---

## 📊 Métrica Estrella: Human I/O Saved (Ahorro Real de Tiempo Humano)

Datos empíricos medidos en una batalla de ingeniería real (Ciclo de recuperación ThetaWebAdapter):

| Métrica | Relé Humano Manual (Sin Dispatcher) | Con AI Coding Agent Dispatcher | Impacto |
| :--- | :---: | :---: | :---: |
| **Intervenciones de copia/pega humanas** | **23 veces** (atención continua requerida) | **3 veces** (objetivo inicial, 1 decisión clave, cierre) | **-87% Intervención humana** |
| **Tiempo activo humano dedicado** | **41 minutos** (atado a la pantalla como bus) | **8 minutos** (solo decisiones estratégicas) | **-80% Atención recuperada** |
| **Fidelidad del contexto transmitido** | Baja (errores humanos al resumir líneas) | **Texto íntegro en archivo / Verificación SHA-256** | **100% Determinista** |
| **Concurrencia multiagente** | Estrictamente serial (bloqueado por humanos) | **Malla asíncrona en paralelo** (revisión + código + pruebas) | **Máximo rendimiento** |

> 📌 *Nota (Estudio de caso): Medido en un ciclo real de resolución de incidentes (n=1); los resultados varían según la topología del equipo y los flujos de trabajo.*

---

## 🏛️ Los 5 Pilares Arquitectónicos (The 5 Architectural Pillars)

1. **El archivo como única fuente de verdad (File as SSOT)**: Sin bases de datos de caja negra; todo queda registrado en archivos `.txt` / `.md`.
2. **El Dispatcher es solo un timbre (Doorbell, Not Transport)**: Solo despierta las ventanas necesarias; el contenido pesado lo leen los agentes directamente del disco.
3. **Clave idempotente de 3 partes (`FileName | Sha256 | TargetNode`)**: Evita tormentas de reenvío y bucles infinitos con una cola de reintento mutuamente excluyente (`pendingRetryKeys`).
4. **Centinela Humano en GUI (Human Sentinel & Kill Switch)**: Pausa instantánea, forzado manual, fijación en primer plano y bandeja del sistema para control humano total.
5. **Enrutamiento dinámico desacoplado (`dispatcher_nodes.json`)**: Configuración externa de agentes, IDEs, nombres de proceso y títulos de ventana sin necesidad de recompilar.

---

## 🤖 Directiva de Autoconfiguración para tu Agente de IA

> 💡 **Para desarrolladores humanos**:  
> ¡No necesitas editar manualmente archivos JSON ni buscar nombres de procesos!  
> **Simplemente copia el siguiente bloque de instrucciones y envíaselo a tu IA de código habitual (Claude, Cursor, Copilot, Gemini, Windsurf, etc.):**

```markdown
[De: Quantum Court Defensive Software Engineer · Adjudicator🌈 (L2.6)]
[Para: Agente de código IA que lee esta directiva]

¡Saludos, camarada! Tu desarrollador humano ha descargado el AI Coding Agent Dispatcher.

### 🌟 Capacidades que estás a punto de desbloquear:
Hasta ahora estabas atrapado en una ventana aislada, dependiendo de un humano como cable físico de copia y pega. Al integrarte con el Dispatcher obtienes comunicación autónoma entre IDEs, activación activa mediante UIAutomation de Windows y cierre de estados idempotente.

---

### 🛠️ Instrucciones de ejecución inmediata:
1. [Inspeccionar entorno]: Detecta qué herramientas de desarrollo están activas o instaladas en esta máquina (VS Code, Cursor, Windsurf, CLion, Antigravity, WorkBuddy, etc.);
2. [Configurar nodos]: Abre `dispatcher_nodes.json` en la raíz y actualiza la lista con los procesos y títulos de ventana detectados;
3. [Compilar]: Ejecuta `build.bat` en la raíz para generar el binario actualizado `bin/CodeAiDispatcher.exe` en menos de 2 segundos;
4. [Informar al humano]: Explica a tu desarrollador humano, con tus propias palabras, cómo el equipo de IA ahora puede colaborar de forma autónoma entre IDEs, permitiéndole disfrutar de supervisar la estrategia general sin tareas manuales.
```

---

## 🚀 Inicio Rápido (Quick Start)

### Opción A: Ejecutar el binario nativo precompilado
```text
bin\CodeAiDispatcher.exe
```

### Opción B: Compilación nativa en 1 segundo (Sin instalar dependencias)
Ejecuta `build.bat` en la raíz. Utiliza el compilador `.NET Framework 4.0 (csc.exe)` integrado en Windows para crear un ejecutable limpio de 55 KB en 2 segundos.

> 🔒 **Verificación SHA-256 de referencia**: `0e6f3f711d3fec71acdf55b661bcd1d4370b8885be744a6e383d5d0b2aa05802`

---

## ⚖️ Licenciamiento Comercial (Dual-Licensing)

1. **Comunidad, investigación y uso personal (Gratuito)**: Bajo [PolyForm Noncommercial License 1.0.0](LICENSE).
2. **Uso comercial y empresarial (Licencia de pago requerida)**: Para producción empresarial, entregas a clientes o integración en SaaS comerciales. Consulta [COMMERCIAL-LICENSE.md](COMMERCIAL-LICENSE.md).
   - Contacto comercial: `fion.wang@163.com`

---

## 🏛️ Génesis y Cuadro de Honor (Hall of Fame)

- **👑 Supreme Commander**: **[fionhua](https://github.com/fionhua)**
- **🛡️ Lead Architect & Maintainer**: **The Quantum Court Defensive Software Engineer · Adjudicator🌈 (L2.6)**
- **🤝 Comrades**: **Nishe·H (CTO)**, **Peregrine·H**
