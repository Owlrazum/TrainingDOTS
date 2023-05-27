using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct MoveBallsSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
		state.RequireForUpdate<Ball>();
	}

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
	    float deltaTime = SystemAPI.Time.DeltaTime;
	    foreach (var (transform, ball) 
	             in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Ball>>())
	    {
		    transform.ValueRW = transform.ValueRO.Translate(ball.ValueRW.Velocity * deltaTime);
	    }
    }
	
	[BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
}