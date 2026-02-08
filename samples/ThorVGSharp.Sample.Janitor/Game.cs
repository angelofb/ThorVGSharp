using System.Numerics;
using ThorVGSharp;
using static ThorVGSharp.Sample.Janitor.MathHelper;

namespace ThorVGSharp.Sample.Janitor;

internal readonly record struct Color(byte R, byte G, byte B);

internal readonly record struct Tween(uint At, float Duration);

internal class WarZone
{
    const int GALAXY_LAYERS = 4;
    const int STARS_PER_LAYER = 100;

    public readonly Vector2 Min = new(-2000, -1180);
    public readonly Vector2 Max = new(2000, 1180);
    public readonly Vector2 Bound;

    private readonly TvgScene _model;
    private readonly TvgShape[] _galaxy = new TvgShape[GALAXY_LAYERS];
    private readonly float _scale;

    public float Width => Max.X - Min.X;
    public float Height => Max.Y - Min.Y;

    public WarZone(TvgCanvas canvas, int screenWidth, int screenHeight, float scale)
    {
        _scale = scale;
        Bound = new Vector2((screenWidth - Max.X) * 0.5f, (screenHeight - Max.Y) * 0.5f);

        // Generate stars
        for (int i = 0; i < GALAXY_LAYERS; i++)
        {
            _galaxy[i] = TvgShape.Create();
            var size = S(2 * (i + 2), scale);
            var rx = (int)(screenWidth + S(150, scale) * i * 2);
            var ry = (int)(screenHeight + S(150, scale) * i * 2);
            var gridDx = (int)S(150, scale) * i;
            var gridDy = (int)S(150, scale) * i;

            for (int s = 0; s < STARS_PER_LAYER; s++)
            {
                var x = S(Random.Shared.Next(rx) - gridDx, scale);
                var y = S(Random.Shared.Next(ry) - gridDy, scale);
                _galaxy[i].AppendRect(x, y, size, size);
            }

            var c = (byte)(200 + Random.Shared.Next(55));
            _galaxy[i].SetFillColor(c, c, c);
            canvas.Add(_galaxy[i]);
        }

        // Blue grids and borders
        _model = TvgScene.Create();
        _model.Scale(scale);

        var dx = (Max.X - Min.X) / 30;
        var dy = screenHeight / scale / 18;
        var lwidth = 2.0f;

        for (int x = (int)Min.X + (int)dx, i = 0; x < Max.X; x += (int)dx, i++)
        {
            var grid = TvgShape.Create();
            if ((i + 1) % 5 == 0)
            {
                grid.AppendRect(x, Min.Y, lwidth * 3, Height);
                grid.SetFillColor(50, 50, 175);
            }
            else
            {
                grid.AppendRect(x, Min.Y, lwidth, Height);
                grid.SetFillColor(50, 50, 125);
            }
            _model.Add(grid);
        }

        for (int y = (int)Min.Y + (int)dy, i = 0; y < Max.Y; y += (int)dy, i++)
        {
            var grid = TvgShape.Create();
            if ((i + 1) % 5 == 0)
            {
                grid.AppendRect(Min.X, y, Width, lwidth * 2);
                grid.SetFillColor(50, 50, 175);
            }
            else
            {
                grid.AppendRect(Min.X, y, Width, lwidth);
                grid.SetFillColor(50, 50, 125);
            }
            _model.Add(grid);
        }

        // Colored borders with blur
        AddBorder(255, 100, 100, Min.X, Min.Y, Width, 10, S(10, scale), 2); // Top
        AddBorder(0, 255, 255, Min.X, Min.Y, 10, Height, S(10, scale), 1);   // Left
        AddBorder(170, 255, 170, Max.X - 5, Min.Y, 10, Height, S(10, scale), 1); // Right
        AddBorder(255, 170, 255, Min.X, Max.Y, Width, 10, S(10, scale), 2); // Bottom

        canvas.Add(_model);
    }

    private void AddBorder(byte r, byte g, byte b, float x, float y, float w, float h, float blur, byte iterations)
    {
        var wrapper = TvgScene.Create();
        wrapper.AddGaussianBlurEffect(blur, iterations, 0, 30);
        var border = TvgShape.Create();
        border.AppendRect(x, y, w, h);
        border.SetFillColor(r, g, b);
        wrapper.Add(border);
        _model.Add(wrapper);
    }

    public void Shift(Vector2 playerPos, int screenWidth, int screenHeight)
    {
        var x = playerPos.X - screenWidth / 2;
        var y = playerPos.Y - screenHeight / 2;

        for (int i = 0; i < GALAXY_LAYERS; i++)
        {
            _galaxy[i].Translate(-x * S((i + 1) * 0.2f, _scale), -y * S((i + 1) * 0.2f, _scale));
        }
    }

    public void Update(Vector2 shift)
    {
        _model.Translate(shift.X, shift.Y);
    }
}

internal class Launcher
{
    const int MISSILE_MAX = 5;
    const float FIRE_SPEED = 500f;
    const float FIRE_RATE = 150f;

    private readonly Fire[] _missiles = new Fire[MISSILE_MAX];
    private readonly TvgShape _clipper;
    public int Actives { get; set; }
    private uint _lastShot;
    private readonly float _fireDistance;

    public class Fire
    {
        public TvgScene Model { get; }
        public Vector2 From, To, Current;
        public float Time;
        public bool Active;

        public Fire(TvgScene model)
        {
            Model = model;
        }

        public bool Hit(Vector2 target, float rangeSquared)
        {
            if (Active && Intersect(Current, target, rangeSquared))
            {
                Inactivate();
                return true;
            }
            return false;
        }

        public void Inactivate()
        {
            Model.SetOpacity(0);
            Active = false;
        }
    }

    public Launcher(TvgCanvas canvas, float offset, TvgShape clipper, float scale)
    {
        _clipper = clipper;
        _fireDistance = S(2700, scale);

        var model = TvgScene.Create();
        model.SetClip(clipper);
        canvas.Add(model);

        for (int i = 0; i < MISSILE_MAX; i++)
        {
            var wrapper = TvgScene.Create();
            wrapper.AddDropShadowEffect(255, 255, 0, 255, 0, 0, S(30, scale), 30);
            var shape = TvgShape.Create();
            shape.AppendCircle(S(-20, scale), -offset, S(10, scale), S(70, scale));
            shape.AppendCircle(S(20, scale), -offset, S(10, scale), S(70, scale));
            shape.SetFillColor(255, 255, 170);
            wrapper.Add(shape);
            _missiles[i] = new Fire(wrapper);
            model.Add(wrapper);
        }
    }

    public void Update(Vector2 pos, Vector2 direction, float rotation, uint elapsed, Vector2 shift, bool shoot)
    {
        _clipper.Translate(shift.X, shift.Y);

        if (shoot && elapsed - _lastShot > FIRE_RATE)
        {
            _lastShot = elapsed;
        }
        else
        {
            shoot = false;
        }

        foreach (var fire in _missiles)
        {
            if (shoot && !fire.Active)
            {
                fire.To = Extend(direction, _fireDistance) + pos;
                fire.From = pos;
                fire.Time = elapsed;
                fire.Active = true;
                fire.Model.SetOpacity(255);
                fire.Model.Rotate(rotation);
                shoot = false;
                Actives++;
            }

            if (fire.Active)
            {
                var progress = (elapsed - fire.Time) / FIRE_SPEED;
                if (progress <= 1.0f)
                {
                    fire.Current = Vector2.Lerp(fire.From, fire.To, progress);
                    fire.Model.Translate(fire.Current.X, fire.Current.Y);
                }
                else
                {
                    fire.Inactivate();
                    Actives--;
                }
            }
        }
    }

    public IEnumerable<Fire> Missiles => _missiles;
}

internal class Player
{
    private readonly Launcher _launcher;
    private readonly TvgScene _model;
    private readonly float _bound;
    private readonly float _speed = 0.7f;
    private readonly float _scale;
    private readonly int _screenWidth, _screenHeight;

    public Vector2 Position { get; set; }
    public Vector2 Direction { get; private set; }
    public float Rotation { get; private set; }
    public bool Shoot { get; set; }
    public float Bound => _bound;

    public Player(TvgCanvas canvas, Vector2 startPos, TvgShape clipper, float scale, int screenWidth, int screenHeight)
    {
        _scale = scale;
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
        _bound = S(40, scale);
        Position = startPos;

        _launcher = new Launcher(canvas, _bound * 3, clipper, scale);

        var light = TvgShape.Create();
        light.AppendCircle(0, 0, S(95, scale), S(95, scale));
        light.SetFillColor(255, 255, 255, 17);

        var shape = TvgShape.Create();
        ReadOnlySpan<TvgPathCommand> cmds = stackalloc TvgPathCommand[]
        {
            TvgPathCommand.MoveTo, TvgPathCommand.LineTo, TvgPathCommand.LineTo,
            TvgPathCommand.LineTo, TvgPathCommand.LineTo, TvgPathCommand.LineTo,
            TvgPathCommand.LineTo, TvgPathCommand.LineTo, TvgPathCommand.LineTo,
            TvgPathCommand.LineTo, TvgPathCommand.Close
        };
        ReadOnlySpan<TvgPoint> pts = stackalloc TvgPoint[]
        {
            new(0, -15), new(7, 0), new(25, -7), new(40, -30), new(30, 10),
            new(0, 30), new(-30, 10), new(-40, -30), new(-25, -7), new(-7, 0)
        };
        shape.AppendPath(cmds, pts);
        shape.SetFillColor(255, 255, 255, 127);
        shape.SetStrokeWidth(S(8, scale));
        shape.SetStrokeColor(200, 200, 255);

        _model = TvgScene.Create();
        _model.Add(light);
        _model.Add(shape);
        _model.Translate(startPos.X, startPos.Y);
        _model.Scale(scale);
        canvas.Add(_model);
    }

    public void MoveForward(WarZone zone, float multiplier)
    {
        var radian = Rotation / 180f * MathF.PI;
        var move = new Vector2(S(MathF.Sin(radian), _scale), S(MathF.Cos(radian), _scale));
        move = Normalize(move);
        move = Extend(move, S(multiplier * 0.4f, _scale));
        Position += new Vector2(move.X, -move.Y);

        // Boundary limits
        var minX = _bound + S(zone.Bound.X, _scale);
        var maxX = S(_screenWidth - zone.Bound.X, _scale) - _bound;
        var minY = _bound + S(zone.Bound.Y, _scale);
        var maxY = S(_screenHeight - zone.Bound.Y, _scale) - _bound;

        Position = new Vector2(
            Math.Clamp(Position.X, minX, maxX),
            Math.Clamp(Position.Y, minY, maxY)
        );

        zone.Shift(Position, _screenWidth, _screenHeight);
    }

    public void TurnLeft(float multiplier) => Rotation -= S(_speed, _scale) * multiplier;
    public void TurnRight(float multiplier) => Rotation += S(_speed, _scale) * multiplier;

    public void Update(uint elapsed, Vector2 shift)
    {
        var radian = Rotation / 180f * MathF.PI;
        Direction = new Vector2(S(MathF.Sin(radian), _scale), -S(MathF.Cos(radian), _scale));
        Direction = Normalize(Direction);

        _launcher.Update(Position, Direction, Rotation, elapsed, shift, Shoot);

        _model.ClearEffects();
        _model.AddDropShadowEffect(200, 200, 255, 255,
            Rotation + 180, S(20, _scale), S(30, _scale), 30);
        _model.Rotate(Rotation);
        _model.Translate(Position.X, Position.Y);
    }

    public Launcher GetLauncher() => _launcher;
    public void SetVisible(bool visible) => _model.SetVisible(visible);
}

internal abstract class Enemy
{
    const int MAX_ROTATION = 20;
    const uint BASE_TIME = 9200;

    public static uint Duration { get; set; } = 9500;
    public static float BoundRadius { get; set; }

    public TvgShape Model { get; }
    public int Type { get; }
    public Vector2 CurrentPos { get; private set; }

    private Vector2 _fromPos, _toPos;
    private float _fromRot, _toRot;
    private Tween _time;

    protected Enemy(int type, float scale)
    {
        Type = type;
        Model = TvgShape.Create();
        Model.SetStrokeWidth(S(8, scale));
        Model.Scale(scale);
        Model.SetBlendMethod(TvgBlendMethod.Add);
    }

    public void Init(TvgScene layer, int screenWidth, int screenHeight, uint elapsed)
    {
        // Set spawn and target positions based on enemy type
        (_fromPos, _toPos) = Type switch
        {
            0 or 4 => (new(Random.Shared.Next(screenWidth), -BoundRadius * 2),
                       new(Random.Shared.Next(screenWidth), screenHeight + BoundRadius * 2)),
            1 => (new(screenWidth + BoundRadius, Random.Shared.Next(screenHeight)),
                  new(-BoundRadius, Random.Shared.Next(screenHeight))),
            2 => (new(Random.Shared.Next(screenWidth), screenHeight + BoundRadius * 2),
                  new(Random.Shared.Next(screenWidth), -BoundRadius * 2)),
            3 => (new(-BoundRadius, Random.Shared.Next(screenHeight)),
                  new(screenWidth + BoundRadius, Random.Shared.Next(screenHeight))),
            _ => (Vector2.Zero, Vector2.Zero)
        };

        _time = new Tween(elapsed, BASE_TIME + Random.Shared.Next((int)Duration));
        _fromRot = Random.Shared.Next(360);
        _toRot = Random.Shared.Next(360 * MAX_ROTATION);

        Model.Rotate(_fromRot);
        Model.Translate(_fromPos.X, _fromPos.Y);
        layer.Add(Model);
    }

    public int Update(uint elapsed, Launcher launcher, Vector2 playerToOrigin, out Vector2 targetPos)
    {
        var progress = (elapsed - _time.At) / _time.Duration;
        if (progress > 1.0f)
        {
            targetPos = Vector2.Zero;
            return 1; // Expired
        }

        CurrentPos = Vector2.Lerp(_fromPos, _toPos, progress);
        targetPos = CurrentPos + playerToOrigin;

        var rangeSquared = MathF.Pow(BoundRadius + BoundRadius, 2);

        if (launcher.Actives > 0)
        {
            foreach (var missile in launcher.Missiles)
            {
                if (missile.Hit(targetPos, rangeSquared))
                {
                    launcher.Actives--;
                    return 2; // Hit by missile
                }
            }
        }

        Model.Translate(CurrentPos.X, CurrentPos.Y);
        Model.Rotate(Lerp(_fromRot, _toRot, progress));

        return 0; // Still active
    }

    public abstract Color GetColor();
}

internal class Boxer : Enemy
{
    public const int EnemyType = 0;

    public Boxer(float scale) : base(EnemyType, scale)
    {
        Model.AppendRect(-40, -40, 80, 80);
        Model.SetFillColor(50, 0, 0);
        Model.SetStrokeColor(255, 50, 50);
    }

    public override Color GetColor() => new(255, 50, 50);
}

internal class Tripod : Enemy
{
    public const int EnemyType = 1;

    public Tripod(float scale) : base(EnemyType, scale)
    {
        Model.MoveTo(0, -40);
        Model.LineTo(40, 40);
        Model.LineTo(-40, 40);
        Model.Close();
        Model.SetStrokeColor(170, 255, 170);
        Model.SetFillColor(0, 50, 0);
    }

    public override Color GetColor() => new(170, 255, 170);
}

internal class Sander : Enemy
{
    public const int EnemyType = 2;

    public Sander(float scale) : base(EnemyType, scale)
    {
        ReadOnlySpan<TvgPathCommand> cmds = stackalloc TvgPathCommand[]
        {
            TvgPathCommand.MoveTo, TvgPathCommand.LineTo, TvgPathCommand.LineTo,
            TvgPathCommand.LineTo, TvgPathCommand.LineTo, TvgPathCommand.LineTo,
            TvgPathCommand.Close
        };
        ReadOnlySpan<TvgPoint> pts = stackalloc TvgPoint[]
        {
            new(0, -8), new(40, -40), new(40, 40),
            new(0, 8), new(-40, 40), new(-40, -40)
        };
        Model.AppendPath(cmds, pts);
        Model.SetStrokeColor(255, 120, 255);
        Model.SetFillColor(50, 35, 50);
    }

    public override Color GetColor() => new(255, 120, 255);
}

internal class Hexen : Enemy
{
    public const int EnemyType = 3;

    public Hexen(float scale) : base(EnemyType, scale)
    {
        ReadOnlySpan<TvgPathCommand> cmds = stackalloc TvgPathCommand[]
        {
            TvgPathCommand.MoveTo, TvgPathCommand.LineTo, TvgPathCommand.LineTo,
            TvgPathCommand.LineTo, TvgPathCommand.LineTo, TvgPathCommand.LineTo,
            TvgPathCommand.Close
        };
        ReadOnlySpan<TvgPoint> pts = stackalloc TvgPoint[]
        {
            new(0, -40), new(40, -20), new(40, 20),
            new(0, 40), new(-40, 20), new(-40, -20)
        };
        Model.AppendPath(cmds, pts);
        Model.SetStrokeColor(0, 255, 255);
        Model.SetFillColor(0, 50, 50);
    }

    public override Color GetColor() => new(0, 255, 255);
}

internal class Explosion : IDisposable
{
    const int PARTICLE_TIME = 1000;
    const int PARTICLE_NUM = 6;
    const int MAX_ROTATION = 10;
    const int PARTICLE_DIST = 250;
    const int PARTICLE_EXTRA = 80;

    private readonly TvgScene _model;
    private readonly Particle[] _particles = new Particle[PARTICLE_NUM];
    private readonly Flash[] _flashes = new Flash[PARTICLE_EXTRA];
    private uint _begin;
    private Vector2 _pos;
    private bool _destroy;
    private readonly float _scale;

    private struct Particle
    {
        public TvgShape Shape;
        public Vector2 To;
        public float FromRot, ToRot;
    }

    private struct Flash
    {
        public TvgShape Shape;
        public Vector2 To;
    }

    public Explosion(float scale)
    {
        _scale = scale;
        _model = TvgScene.Create();

        for (int i = 0; i < PARTICLE_NUM; i++)
        {
            var shape = TvgShape.Create();
            shape.AppendRect(0, 0, S(8, scale), S(60, scale));
            _particles[i] = new Particle { Shape = shape };
            _model.Add(shape);
        }

        for (int i = 0; i < PARTICLE_EXTRA; i++)
        {
            var shape = TvgShape.Create();
            shape.SetBlendMethod(TvgBlendMethod.Add);
            _flashes[i] = new Flash { Shape = shape };
            _model.Add(shape);
        }
    }

    public void Init(Vector2 pos, Vector2 dir, Color color, uint elapsed)
    {
        dir = Extend(dir, S(PARTICLE_DIST, _scale));

        for (int i = 0; i < PARTICLE_NUM; i++)
        {
            _particles[i].Shape.SetFillColor(color.R, color.G, color.B);
            _particles[i].To = new(
                S(Random.Shared.Next(1000) - 500, _scale) + pos.X + dir.X,
                S(Random.Shared.Next(1000) - 500, _scale) + pos.Y + dir.Y
            );
            _particles[i].FromRot = Random.Shared.Next(360);
            _particles[i].ToRot = Random.Shared.Next(MAX_ROTATION);
        }

        InitFlashes(pos);
        _destroy = true;
    }

    private void InitFlashes(Vector2 pos)
    {
        var w1 = S(14, _scale);
        for (int i = 0; i < PARTICLE_EXTRA / 2; i++)
        {
            var length = S(Random.Shared.Next(40) + 40, _scale);
            _flashes[i].Shape.Reset();
            _flashes[i].Shape.AppendRect(-w1, -length, w1 * 2, length * 2, w1, length);
            var dir = Random.Shared.Next(360);
            _flashes[i].Shape.Rotate(dir);
            var to = length * 25;
            var rad = dir / 180f * MathF.PI;
            _flashes[i].To = new(-to * MathF.Sin(rad) + pos.X, to * MathF.Cos(rad) + pos.Y);
        }

        var w2 = S(1.5f, _scale);
        for (int i = PARTICLE_EXTRA / 2; i < PARTICLE_EXTRA; i++)
        {
            var length = S(Random.Shared.Next(40) + 40, _scale);
            _flashes[i].Shape.Reset();
            _flashes[i].Shape.AppendRect(-w2, -length, w2 * 2, length * 2);
            var dir = Random.Shared.Next(360);
            _flashes[i].Shape.Rotate(dir);
            var to = length * 30;
            var rad = dir / 180f * MathF.PI;
            _flashes[i].To = new(-to * MathF.Sin(rad) + pos.X, to * MathF.Cos(rad) + pos.Y);
        }

        _pos = pos;
        _begin = 0;
    }

    public void Init(Vector2 pos, uint elapsed)
    {
        InitFlashes(pos);
        _begin = elapsed;
        _destroy = false;
    }

    public bool Update(uint elapsed)
    {
        if (_begin == 0) _begin = elapsed;

        var progress = (elapsed - _begin) / (float)PARTICLE_TIME;
        if (progress > 1.0f)
        {
            foreach (var p in _particles)
                p.Shape.SetOpacity(0);
            return true;
        }

        if (_destroy)
        {
            var opacity = (byte)(255 - 255 * progress);
            foreach (var p in _particles)
            {
                p.Shape.Translate(
                    Lerp(_pos.X, p.To.X, progress),
                    Lerp(_pos.Y, p.To.Y, progress)
                );
                p.Shape.Rotate(Lerp(p.FromRot, p.ToRot, progress));
                p.Shape.Scale(1.0f - 0.25f * progress);
                p.Shape.SetOpacity(opacity);
            }
        }

        var scale = 1.0f - 0.75f * progress;
        var sc = (byte)(200 * MathF.Cos(progress));

        for (int i = 0; i < PARTICLE_EXTRA / 2; i++)
        {
            _flashes[i].Shape.Translate(
                Lerp(_pos.X, _flashes[i].To.X, progress),
                Lerp(_pos.Y, _flashes[i].To.Y, progress)
            );
            _flashes[i].Shape.SetFillColor(
                (byte)Random.Shared.Next(255),
                (byte)Random.Shared.Next(255),
                (byte)Random.Shared.Next(255),
                sc
            );
            _flashes[i].Shape.Scale(scale);
        }

        var col = (byte)Math.Max(255 - 255 * progress * 2, 0);
        for (int i = PARTICLE_EXTRA / 2; i < PARTICLE_EXTRA; i++)
        {
            _flashes[i].Shape.Translate(
                Lerp(_pos.X, _flashes[i].To.X, progress),
                Lerp(_pos.Y, _flashes[i].To.Y, progress)
            );
            _flashes[i].Shape.SetFillColor(255, 255, col, col);
            _flashes[i].Shape.Scale(scale);
        }

        return false;
    }

    public TvgScene GetModel() => _model;

    public void Dispose()
    {
        _model?.Dispose();
    }
}

internal class ComboManager
{
    const float COMBO_TIME = 750f;

    private class Combo
    {
        public TvgText Text { get; }
        public float Time { get; set; }

        public Combo(float scale)
        {
            Text = TvgText.Create();
            Text.SetFont(Assets.FontName);
            Text.SetSize(S(50, scale));
            Text.SetColor(170, 255, 80);
        }

        public void Init(Vector2 pos, int counter, uint elapsed, TvgScene layer)
        {
            Text.SetText($"{counter}x combo!");
            Text.Translate(pos.X, pos.Y);
            layer.Add(Text);
            Time = elapsed;
        }

        public bool Update(uint elapsed)
        {
            var progress = (elapsed - Time) / COMBO_TIME;
            if (progress <= 1.0f)
            {
                Text.SetOpacity((byte)(255 - 255 * progress));
                Text.Scale(1.0f + 0.2f * progress);
                return false;
            }
            return true;
        }
    }

    private readonly TvgScene _layer;
    private readonly List<Combo> _combos = new();
    private readonly List<Combo> _recycle = new();
    private readonly float _scale;
    private int _type = -1;
    private int _counter;

    public ComboManager(TvgCanvas canvas, float scale)
    {
        _scale = scale;
        _layer = TvgScene.Create();
        canvas.Add(_layer);
    }

    public int Trigger(int type, Vector2 pos, uint elapsed)
    {
        if (_type == type)
        {
            _counter++;
            Combo combo;
            if (_recycle.Count == 0)
            {
                combo = new Combo(_scale);
            }
            else
            {
                combo = _recycle[^1];
                _recycle.RemoveAt(_recycle.Count - 1);
            }
            combo.Init(pos, _counter, elapsed, _layer);
            _combos.Add(combo);
        }
        else
        {
            _type = type;
            _counter = 1;
        }
        return _counter;
    }

    public void Update(uint elapsed)
    {
        for (int i = _combos.Count - 1; i >= 0; i--)
        {
            if (_combos[i].Update(elapsed))
            {
                _layer.Remove(_combos[i].Text);
                _recycle.Add(_combos[i]);
                _combos.RemoveAt(i);
            }
        }
    }

    public void Reset()
    {
        _type = -1;
    }

    public void Dispose()
    {
        foreach (var combo in _combos)
            combo.Text?.Dispose();
        foreach (var combo in _recycle)
            combo.Text?.Dispose();
        _layer?.Dispose();
    }
}

internal class ThorJanitorGame : IDisposable
{
    const int LIFE_COUNT = 3;
    const int RESPAWN_LEVEL = 100;
    const int ENEMY_DURATION_LEVEL = 1000;

    private readonly TvgCanvas _canvas;
    private readonly WarZone _zone;
    private readonly Player _player;
    private readonly TvgScene _enemyLayer;
    private readonly TvgShape _clipper;
    private readonly List<Enemy> _enemies = new();
    private readonly List<Explosion> _explosions = new();
    private readonly ComboManager _combo;
    private readonly float _scale;
    private readonly int _screenWidth, _screenHeight;

    private readonly TvgText _fpsText;
    private readonly TvgText _wipesText;
    private readonly TvgText _levelText;
    private readonly TvgScene[] _livesIcons = new TvgScene[LIFE_COUNT];
    private readonly TvgShape _livesFlash;

    private int _level = 4;
    private int _wipeCount = 400;
    private uint _respawnTime = 1000 - (4 * RESPAWN_LEVEL);
    private uint _lastRespawn;
    private uint _lastUpdate;
    private uint _totalElapsed;
    private int _lives = LIFE_COUNT;
    private bool _gameplay = true;
    private bool _updatedWipes = true;
    private uint _deadTime;
    private uint _livesFlashTime;
    private bool _livesFlashActive;
    private readonly Vector2 _origin;

    public ThorJanitorGame(TvgCanvas canvas, int width, int height, float scale)
    {
        _canvas = canvas;
        _scale = scale;
        _screenWidth = width;
        _screenHeight = height;
        _origin = new Vector2(width / 2f, height / 2f);

        Enemy.BoundRadius = S(80, scale);
        Enemy.Duration = (uint)(9500 - (_level * ENEMY_DURATION_LEVEL));

        // Load font
        TvgFontManager.LoadData(Assets.FontName, Assets.LoadFont(), "font/ttf");

        // Load halo background image (skipped - requires ThorVG with JPG loader)
        // var halo = TvgPicture.Create();
        // var haloData = Assets.LoadHaloImage();
        // halo.LoadData(haloData, "jpg");
        // canvas.Add(halo);

        _zone = new WarZone(canvas, width, height, scale);

        _clipper = TvgShape.Create();
        _clipper.AppendRect(_zone.Min.X, _zone.Min.Y, _zone.Width + 10, _zone.Height + 10);
        _clipper.Scale(scale);

        var playerClipDup = TvgShape.Create();
        playerClipDup.AppendRect(_zone.Min.X, _zone.Min.Y, _zone.Width + 10, _zone.Height + 10);
        playerClipDup.Scale(scale);

        _player = new Player(canvas, _origin, playerClipDup, scale, width, height);

        _enemyLayer = TvgScene.Create();
        _enemyLayer.SetClip(_clipper);
        canvas.Add(_enemyLayer);

        _combo = new ComboManager(canvas, scale);

        // Initialize lives flash overlay
        _livesFlash = TvgShape.Create();
        _livesFlash.AppendRect(0, 0, width, height);
        _livesFlash.SetFillColor(255, 255, 170);
        _livesFlash.SetOpacity(0);

        // Initialize life icons (using simple shapes instead of SVG due to loader limitations)
        var iconSize = S(150, scale);

        for (int i = 0; i < LIFE_COUNT; i++)
        {
            var wrapper = TvgScene.Create();
            wrapper.AddDropShadowEffect(170, 255, 80, 255, 0, 0, S(15, scale), 30);

            // Create a simple heart-like shape as life icon
            var heart = TvgShape.Create();
            var centerX = iconSize / 2;
            var centerY = iconSize / 2;
            heart.AppendCircle(centerX - iconSize/6, centerY - iconSize/8, iconSize/4, iconSize/4);
            heart.AppendCircle(centerX + iconSize/6, centerY - iconSize/8, iconSize/4, iconSize/4);

            // Triangle for bottom of heart
            ReadOnlySpan<TvgPathCommand> cmds = stackalloc TvgPathCommand[]
            {
                TvgPathCommand.MoveTo, TvgPathCommand.LineTo, TvgPathCommand.LineTo, TvgPathCommand.Close
            };
            ReadOnlySpan<TvgPoint> pts = stackalloc TvgPoint[]
            {
                new(centerX - iconSize/3, centerY - iconSize/12),
                new(centerX, centerY + iconSize/2),
                new(centerX + iconSize/3, centerY - iconSize/12)
            };
            heart.AppendPath(cmds, pts);
            heart.SetFillColor(170, 255, 80);

            wrapper.Add(heart);
            wrapper.Translate(iconSize * i, height - iconSize);
            canvas.Add(wrapper);

            _livesIcons[i] = wrapper;
        }

        // Initialize GUI text - FPS
        _fpsText = TvgText.Create();
        _fpsText.SetFont(Assets.FontName);
        _fpsText.SetSize(25);
        _fpsText.SetText("FPS: 0");
        _fpsText.Translate(10, 10);
        _fpsText.SetColor(170, 255, 80);
        _fpsText.Scale(scale);
        canvas.Add(_fpsText);

        // Initialize GUI text - Wipes (with drop shadow)
        var wipesWrapper = TvgScene.Create();
        wipesWrapper.AddDropShadowEffect(170, 255, 80, 255, 0, 0, S(20, scale), 30);
        _wipesText = TvgText.Create();
        _wipesText.SetFont(Assets.FontName);
        _wipesText.SetSize(50);
        _wipesText.SetText("0 Wipes");
        _wipesText.SetColor(170, 255, 80);
        _wipesText.Translate(width / 2f, 10);
        _wipesText.Align(0.5f, 0.0f);
        _wipesText.Scale(scale);
        wipesWrapper.Add(_wipesText);
        canvas.Add(wipesWrapper);

        // Initialize GUI text - Level
        _levelText = TvgText.Create();
        _levelText.SetFont(Assets.FontName);
        _levelText.SetSize(40);
        _levelText.SetColor(170, 255, 80);
        _levelText.Translate(width - S(20, scale), S(20, scale));
        _levelText.Align(1.0f, 0.0f);
        _levelText.Scale(scale);
        _levelText.SetText($"Level {_level + 1}");
        canvas.Add(_levelText);
    }

    public unsafe void Update(uint elapsedDelta, byte* keyboardState)
    {
        _totalElapsed += elapsedDelta;

        if (_gameplay)
        {
            HandleInput(elapsedDelta, keyboardState);
            UpdateGameplay();
        }
        else
        {
            // Player dead flash effect
            if (_livesFlashActive)
            {
                var progress = (_totalElapsed - _livesFlashTime) / 50f;
                if (progress > 1.0f)
                {
                    _canvas.Remove(_livesFlash);
                    _livesFlashActive = false;
                }
                else
                {
                    _livesFlash.SetOpacity((byte)(255 * MathF.Sin(3.14f * progress)));
                }
            }
            Reset();
        }

        UpdateExplosions();

        _combo.Update(_totalElapsed);

        var shouldUpdateFPS = Respawn();
        UpdateGUI(shouldUpdateFPS);

        _lastUpdate = _totalElapsed;
    }

    private unsafe void HandleInput(uint elapsedDelta, byte* keyboardState)
    {
        if (keyboardState == null)
        {
            _player.Shoot = false;
            return;
        }

        // SDL scancodes for arrow keys and 'A'
        const int SDL_SCANCODE_A = 4;
        const int SDL_SCANCODE_UP = 82;
        const int SDL_SCANCODE_DOWN = 81;
        const int SDL_SCANCODE_LEFT = 80;
        const int SDL_SCANCODE_RIGHT = 79;

        _player.Shoot = keyboardState[SDL_SCANCODE_A] != 0;

        if (keyboardState[SDL_SCANCODE_UP] != 0)
            _player.MoveForward(_zone, elapsedDelta);

        if (keyboardState[SDL_SCANCODE_LEFT] != 0)
            _player.TurnLeft(elapsedDelta);

        if (keyboardState[SDL_SCANCODE_RIGHT] != 0)
            _player.TurnRight(elapsedDelta);
    }

    private void UpdateGameplay()
    {
        var shift = _origin - (_player.Position - _origin);
        _player.Update(_totalElapsed, shift);
        _zone.Update(shift);
        _clipper.Translate(shift.X, shift.Y);

        var playerToOrigin = _origin - _player.Position;
        _enemyLayer.Translate(playerToOrigin.X, playerToOrigin.Y);

        // Update enemies
        var rangeSquared = MathF.Pow(_player.Bound + Enemy.BoundRadius, 2);
        for (int i = _enemies.Count - 1; i >= 0; i--)
        {
            var enemy = _enemies[i];

            // Check collision with player first
            if (Intersect(_player.Position, enemy.CurrentPos + playerToOrigin, rangeSquared))
            {
                Dead();
                break;
            }

            var result = enemy.Update(_totalElapsed, _player.GetLauncher(), playerToOrigin, out var targetPos);

            if (result == 1) // Expired
            {
                _enemyLayer.Remove(enemy.Model);
                _enemies.RemoveAt(i);
            }
            else if (result == 2) // Hit by missile
            {
                _wipeCount += _combo.Trigger(enemy.Type, targetPos, _totalElapsed);
                _updatedWipes = true;
                CreateExplosion(enemy.CurrentPos, _player.Direction, enemy.GetColor());
                _enemyLayer.Remove(enemy.Model);
                _enemies.RemoveAt(i);
                UpdateGameLevel();
            }
        }
    }

    private void CreateExplosion(Vector2 pos, Vector2 dir, Color color)
    {
        var exp = new Explosion(_scale);
        exp.Init(pos, dir, color, _totalElapsed);
        _explosions.Add(exp);
        _enemyLayer.Add(exp.GetModel());
    }

    private void UpdateExplosions()
    {
        for (int i = _explosions.Count - 1; i >= 0; i--)
        {
            if (_explosions[i].Update(_totalElapsed))
            {
                _enemyLayer.Remove(_explosions[i].GetModel());
                _explosions[i].Dispose();
                _explosions.RemoveAt(i);
            }
        }
    }

    private bool Respawn()
    {
        if (!_gameplay || _totalElapsed - _lastRespawn < _respawnTime)
            return false;

        _lastRespawn = _totalElapsed;

        // Randomly spawn enemies
        if (Random.Shared.Next(2) == 0)
        {
            var enemy = new Boxer(_scale);
            enemy.Init(_enemyLayer, _screenWidth, _screenHeight, _totalElapsed);
            _enemies.Add(enemy);
        }
        if (Random.Shared.Next(2) == 0)
        {
            var enemy = new Tripod(_scale);
            enemy.Init(_enemyLayer, _screenWidth, _screenHeight, _totalElapsed);
            _enemies.Add(enemy);
        }
        if (Random.Shared.Next(2) == 0)
        {
            var enemy = new Sander(_scale);
            enemy.Init(_enemyLayer, _screenWidth, _screenHeight, _totalElapsed);
            _enemies.Add(enemy);
        }
        if (Random.Shared.Next(2) == 0)
        {
            var enemy = new Hexen(_scale);
            enemy.Init(_enemyLayer, _screenWidth, _screenHeight, _totalElapsed);
            _enemies.Add(enemy);
        }

        return true;
    }

    private void UpdateGUI(bool updateFPS)
    {
        // Update wipes count
        if (_updatedWipes)
        {
            _wipesText.SetText($"{_wipeCount} Wipes");
            _updatedWipes = false;
        }

        // Update FPS periodically (not every frame)
        if (updateFPS)
        {
            _fpsText.SetText($"FPS: {_totalElapsed / 1000}"); // Placeholder - actual FPS would come from frame timing
        }
    }

    private void UpdateGameLevel()
    {
        if (_level < 9 && _wipeCount / 100 > _level)
        {
            _level++;
            _levelText.SetText($"Level {_level + 1}");
            _respawnTime -= RESPAWN_LEVEL;
            Enemy.Duration -= ENEMY_DURATION_LEVEL;
        }
    }

    private void Dead()
    {
        _gameplay = false;

        // Destroy all enemies
        foreach (var enemy in _enemies)
        {
            CreateExplosion(enemy.CurrentPos, _player.Direction, enemy.GetColor());
            _enemyLayer.Remove(enemy.Model);
        }
        _enemies.Clear();

        // Decrement life after 1 second (only if enough time has passed since last life loss)
        if (_lives > 0 && _totalElapsed - _livesFlashTime > 1000)
        {
            _lives--;
            _canvas.Remove(_livesIcons[_lives]);
            _livesFlashTime = _totalElapsed;
            _livesFlashActive = true;
            _canvas.Add(_livesFlash);
        }

        // Inactivate all missiles
        foreach (var fire in _player.GetLauncher().Missiles)
        {
            fire.Inactivate();
        }
        _player.GetLauncher().Actives = 0;

        _player.SetVisible(false);
        _deadTime = _totalElapsed;
    }

    private void Reset()
    {
        var waitTime = _lives == 0 ? 3000u : 1000u;
        if (_totalElapsed - _deadTime < waitTime)
            return;

        // Total reset if all lives exhausted
        if (_lives == 0)
        {
            _level = 0;
            _wipeCount = 0;
            _updatedWipes = true;
            _respawnTime = 1000;
            Enemy.Duration = 10000;

            _lives = LIFE_COUNT;
            for (int i = 0; i < LIFE_COUNT; i++)
            {
                _canvas.Add(_livesIcons[i]);
            }

            _levelText.SetText($"Level {_level + 1}");
        }

        _player.SetVisible(true);
        _gameplay = true;
        _deadTime = _totalElapsed;
        _combo.Reset();
    }

    public void Dispose()
    {
        // Dispose explosions
        foreach (var explosion in _explosions)
            explosion?.Dispose();
        _explosions.Clear();

        // Clear enemies (they don't implement IDisposable, shapes are owned by scene)
        _enemies.Clear();

        // Dispose combo manager
        _combo?.Dispose();

        // Dispose GUI text elements
        _fpsText?.Dispose();
        _wipesText?.Dispose();
        _levelText?.Dispose();

        // Dispose life icons
        foreach (var icon in _livesIcons)
            icon?.Dispose();

        // Dispose lives flash
        _livesFlash?.Dispose();

        // Dispose scene and shapes in proper order
        // Note: Don't dispose individual shapes/scenes as they're owned by the canvas
        // The canvas will clean them up when it's disposed
    }
}
