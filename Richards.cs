using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace richardsCsharp
{
    class Richards : Benchmark
    {
        public static int ID_IDLE = 0;
        public static int ID_WORKER = 1;
        public static int DATA_SIZE = 4;
        public static int ID_HANDLER_A = 2;
        public static int ID_HANDLER_B = 3;
        public static int ID_DEVICE_A = 4;
        public static int ID_DEVICE_B = 5;

        public static int COUNT = 1000;
        public static int EXPECTED_QUEUE_COUNT = 2322;
        public static int EXPECTED_HOLD_COUNT = 928;

        public Richards(string name, bool doWarmup, bool doDeterministic, int deterministicIterations,
            int minIterations)
        {
            this.name = name;
            this.doWarmup = doWarmup;
            this.doDeterministic = doDeterministic;
            this.deterministicIterations = deterministicIterations;
            this.minIterations = minIterations;
        }

        override public void run()
        {
            Scheduler scheduler = new Scheduler();
            scheduler.addIdleTask(ID_IDLE, 0, null, COUNT);

            Packet queue = new Packet(null, ID_WORKER, Packet.Kind.WORK);
            queue = new Packet(queue, ID_WORKER, Packet.Kind.WORK);
            scheduler.addWorkerTask(ID_WORKER, 1000, queue);

            queue = new Packet(null, ID_DEVICE_A, Packet.Kind.DEVICE);
            queue = new Packet(queue, ID_DEVICE_A, Packet.Kind.DEVICE);
            queue = new Packet(queue, ID_DEVICE_A, Packet.Kind.DEVICE);
            scheduler.addHandlerTask(ID_HANDLER_A, 2000, queue);

            queue = new Packet(null, ID_DEVICE_B, Packet.Kind.DEVICE);
            queue = new Packet(queue, ID_DEVICE_B, Packet.Kind.DEVICE);
            queue = new Packet(queue, ID_DEVICE_B, Packet.Kind.DEVICE);
            scheduler.addHandlerTask(ID_HANDLER_B, 3000, queue);

            scheduler.addDeviceTask(ID_DEVICE_A, 4000, null);

            scheduler.addDeviceTask(ID_DEVICE_B, 5000, null);

            scheduler.schedule();

            if (scheduler.queueCount != EXPECTED_QUEUE_COUNT ||
                scheduler.holdCount != EXPECTED_HOLD_COUNT)
            {
                string msg =
                    "Error during execution: queueCount = " + scheduler.queueCount +
                    ", holdCount = " + scheduler.holdCount + ".";
                throw new Exception(msg);
            }
        }

    }
}
